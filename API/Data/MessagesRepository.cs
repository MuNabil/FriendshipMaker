namespace API.Data;

public class MessagesRepository : IMessagesRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public MessagesRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;

    }

    public void AddMessage(Message message)
    {
        _dbContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _dbContext.Messages.Remove(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _dbContext.Messages.Include(m => m.Sender)
            .Include(m => m.Recipient).SingleOrDefaultAsync(m => m.Id == id);
    }

    // Get all messages that I send/recieve them to/from all users or the recieved messages that I did't read it yet.
    public async Task<PagedList<MessageDto>> GetMessagesforUserAsync(MessageParams messageParams)
    {
        // get the messages order by newest
        var query = _dbContext.Messages.OrderByDescending(m => m.SendAt).AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username // Get the messages that I recieve it
                && !m.RecipientDeleted), // and I don't delete it (as a recipient)
            "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username  // Get the messages that I send it
                && !m.SenderDeleted), // and I don't delete it (as a sender)
            _ => query.Where(m => m.Recipient.UserName == messageParams.Username && m.ReadAt == null //Get the revieved unreaded messages
                && !m.RecipientDeleted) // and I don't delete it (as a recipient)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }


    // This method to get a (conversation/chat) of two users
    public async Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername)
    {
        var messages = await _dbContext.Messages
                            .Include(m => m.Sender).ThenInclude(s => s.Photos)  // To get the photos to take the main one in the mapping
                            .Include(m => m.Recipient).ThenInclude(r => r.Photos)
                            // Get the messages that I send them to this user and I don't delete it
                            .Where(m => (m.Sender.UserName == currentUsername && m.Recipient.UserName == recipientUsername && m.SenderDeleted == false)
                                // Get the messages this user send it to me and I don't delete it
                                || (m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false))
                            .OrderBy(m => m.SendAt)
                            .ToListAsync();

        // Get Unreaded messages that sent to me
        var unreadedMessages = messages.Where(m => m.Recipient.UserName == currentUsername && m.ReadAt is null).ToList();
        //Mark them as readed
        if (unreadedMessages.Any())
        {
            foreach (var message in unreadedMessages)
            {
                message.ReadAt = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
        }

        // return the chat
        return _mapper.Map<IEnumerable<MessageDto>>(messages);

    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    //For SignalR
    public void AddGroup(Group group)
    {
        _dbContext.Groups.Add(group);
    }
    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _dbContext.Connections.FindAsync(connectionId);
    }
    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _dbContext.Groups.Include(g => g.Connections).FirstOrDefaultAsync(g => g.Name == groupName);
    }
    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _dbContext.Groups.Include(g => g.Connections)
        .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        _dbContext.Connections.Remove(connection);
    }
}