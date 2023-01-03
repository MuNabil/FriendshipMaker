namespace API.SignalR;

public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;
    public MessageHub(IMessagesRepository messagesRepository, IMapper mapper,
         IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
    {
        _tracker = tracker;
        _presenceHub = presenceHub;
        _userRepository = userRepository;
        _messagesRepository = messagesRepository;
        _mapper = mapper;

    }

    public override async Task OnConnectedAsync()
    {
        // To get the other user's username from the Query string
        // anyone will connect to this hub will send the user in the query string as (messageHubUrl?user=otherUsername)
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();

        // To create a unique group name for every chat between 2-users
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        // To add the users connection to thier group
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        //to tracking the message groups
        var group = await AddToGroup(groupName);

        // "UpdatedGroup" event will send the group and both user connections
        // So If the recipient is in the group so he open the chat So mark the message as readed
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        // Get the chat between these two users
        var messages = await _messagesRepository.GetMessagesThreadAsync(Context.User.GetUsername(), otherUser);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        //to tracking the message groups
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

        // When any user disconnects SignalR will remove it from the group
        await base.OnDisconnectedAsync(exception);
    }


    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        // Get the currentUser which represents the Sender
        var username = Context.User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You can't messageing yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient is null)
            throw new HubException($"User with username '{createMessageDto.RecipientUsername}' not found");

        //Create the message
        var newMessage = new Message
        {
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        // To get the group name that the sender and the recipient belong to
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _messagesRepository.GetMessageGroup(groupName);

        // IF the recipient is online and he is also in the same group(chat) then we will mark it as readed
        if (group.Connections.Any(c => c.Username == recipient.UserName))
        {
            newMessage.ReadAt = DateTime.UtcNow;
        }
        else // To know is the recipient is online but he is not in the group (notify him) or he is offline
        {
            // Get all the recipient connections
            var connections = await _tracker.GetConnectionForUser(recipient.UserName);
            if (connections is not null) // If the the recipient is online but not in the same message group, cuz (he has a connections somewhere else)
            {
                // To send this event as a PresenceHub event so client can listen to it from presenceHub
                await _presenceHub.Clients.Clients(connections)
                    // To send the sender info to notify the recipient
                    .SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });

            }
        }
        _messagesRepository.AddMessage(newMessage);

        if (await _messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(newMessage));
        }
    }

    // These 2-method to tracking the message groups
    // I will return a Group from both 2-methods because I wanna see which user is in the group(online) so I send the messages to him
    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messagesRepository.GetMessageGroup(groupName);
        // To get a new connection from Connection entity that I created
        // so when a user is connects to this hub will given a new connectionId unless the're reconnecting
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        // To add this group to the database if it doesn't already in it
        if (group is null)
        {
            group = new Group(groupName);
            _messagesRepository.AddGroup(group);
        }
        // To add this new connection to its connection collection<Connection>....
        group.Connections.Add(connection);

        if (await _messagesRepository.SaveAllAsync()) return group;

        throw new HubException("Faild to add to group");
    }
    private async Task<Group> RemoveFromMessageGroup()
    {
        // To get the group that containing this connection
        var group = await _messagesRepository.GetGroupForConnection(Context.ConnectionId);
        // To get the connection from the group above becaue I Include it at the query instead of getting it from the repository(dataabase)
        var connection = group.Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
        _messagesRepository.RemoveConnection(connection);

        if (await _messagesRepository.SaveAllAsync()) return group;

        throw new HubException("Faild to remove from group");
    }



    // Method to return a group name that every pair of users belongs to
    private string GetGroupName(string caller, string other)
    {
        // The group name will be the name of 2-users separated by a "-" order by decsinding
        var isCallerSmaller = string.CompareOrdinal(caller, other) < 0;
        return isCallerSmaller ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}