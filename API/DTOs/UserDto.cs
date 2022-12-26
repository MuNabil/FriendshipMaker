namespace API.DTOs;

// This class to send this detail as a response when user login or register
public class UserDto
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public string PhotoUrl { get; set; }
    public string KnownAs { get; set; }
    public string Gender { get; set; }
}