namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    public AccountController(ApplicationDbContext dbContext, ITokenService tokenService, IMapper mapper)
    {
        _mapper = mapper;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("This user is already exists..!");

        var user = _mapper.Map<ApplicationUser>(registerDto);

        using var hmac = new HMACSHA512();

        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return new UserDto { UserName = user.UserName, Token = _tokenService.CreateToken(user), KnownAs = user.KnownAs };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _dbContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName.Equals(loginDto.UserName.ToLower()));

        if (user is null) return Unauthorized("Invalid Information..!");

        var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
            if (!computedHash[i].Equals(user.PasswordHash[i])) return Unauthorized("Invalid Information..!");

        var mainPhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;

        return new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = mainPhotoUrl,
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> UserExists(string username) =>
     await _dbContext.Users.AnyAsync(u => u.UserName.Equals(username.ToLower()));

}
