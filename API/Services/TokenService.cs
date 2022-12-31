namespace API.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<ApplicationUser> _userManager;
    public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["tokenKey"]));
    }
    public async Task<string> CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        // To send the roles of this user with the claim in token
        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(15),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
