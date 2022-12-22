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
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return Ok(await _userRepository.GetMemberByNameAsync(username));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UpdateMemberDto updateMemberDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        // user.Introduction = updateMemberDto.Introduction;
        // user.Interests = updateMemberDto.Interests;
        // user.LookingFor = updateMemberDto.LookingFor;
        // user.City = updateMemberDto.City;
        // user.Country = updateMemberDto.Country;

        _mapper.Map(updateMemberDto, user);

        _userRepository.Update(user);
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user.");
    }
}
