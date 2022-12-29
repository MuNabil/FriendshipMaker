namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;
    public MessagesController(IUserRepository userRepository, IMessagesRepository messagesRepository, IMapper mapper)
    {
        _mapper = mapper;
        _messagesRepository = messagesRepository;
        _userRepository = userRepository;

    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        // Get the currentUser which represents the Sender
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername) return BadRequest("You can't messageing yourself");
        var sender = await _userRepository.GetUserByUsernameAsync(username);

        // get the recipient
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient is null) return NotFound();

        //Create the message
        var newMessage = new Message
        {
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        _messagesRepository.AddMessage(newMessage);

        if (await _messagesRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(newMessage));

        return BadRequest("Failed to send the message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        // this will return the messages as well as the pagination information
        var messages = await _messagesRepository.GetMessagesforUserAsync(messageParams);

        // To send the pagination information in the response header to the client
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        // then send the messages in the body of the response
        return messages;
    }

    [HttpGet("thread/{username}")] // username = recipientUsername
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        var messages = await _messagesRepository.GetMessagesThreadAsync(currentUsername, username);
        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await _messagesRepository.GetMessageAsync(id);

        if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted) _messagesRepository.DeleteMessage(message);

        if (await _messagesRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to delete the message");
    }
}