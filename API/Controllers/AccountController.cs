namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITokenService _tokenService;
    public AccountController(ApplicationDbContext dbContext, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("This user is already exists..!");

        using var hmac = new HMACSHA512();

        var user = new ApplicationUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return new UserDto { UserName = user.UserName, Token = _tokenService.CreateToken(user) };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserName.Equals(loginDto.UserName.ToLower()));

        if (user is null) return Unauthorized("Invalid Information..!");

        var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
            if (!computedHash[i].Equals(user.PasswordHash[i])) return Unauthorized("Invalid Information..!");

        return new UserDto { UserName = user.UserName, Token = _tokenService.CreateToken(user) };
    }

    private async Task<bool> UserExists(string username) =>
     await _dbContext.Users.AnyAsync(u => u.UserName.Equals(username.ToLower()));

}
