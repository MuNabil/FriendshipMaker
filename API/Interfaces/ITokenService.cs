namespace API.Interfaces;
public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}