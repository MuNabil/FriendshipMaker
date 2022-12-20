namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUsers()
    {
        return Ok(await _userRepository.GetMembersAsync());
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUserById(string username)
    {
        return Ok(await _userRepository.GetMemberByNameAsync(username));
    }
}
