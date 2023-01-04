namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        // Get the currentUser which represents the Sender
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername) return BadRequest("You can't messageing yourself");
        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        // get the recipient
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
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
        _unitOfWork.MessageRepository.AddMessage(newMessage);

        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(newMessage));

        return BadRequest("Failed to send the message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        // this will return the messages as well as the pagination information
        var messages = await _unitOfWork.MessageRepository.GetMessagesforUserAsync(messageParams);

        // To send the pagination information in the response header to the client
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        // then send the messages in the body of the response
        return messages;
    }

    [HttpGet("thread/{username}")] // username = recipientUsername
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        var messages = await _unitOfWork.MessageRepository.GetMessagesThreadAsync(currentUsername, username);
        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await _unitOfWork.MessageRepository.GetMessageAsync(id);

        if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to delete the message");
    }
}