namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
         ITokenService tokenService, IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("This user is already exists..!");

        var user = _mapper.Map<ApplicationUser>(registerDto);

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");
        if (!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users.Include(p => p.Photos)
            .SingleOrDefaultAsync(u => u.UserName.Equals(loginDto.UserName.Trim().ToLower()));

        if (user is null) return Unauthorized("Invalid Information..!");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized();

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username) =>
     await _userManager.Users.AnyAsync(u => u.UserName.Equals(username.Trim().ToLower()));

}
