namespace API.DTOs;

// Containing the information that we need to displaly the list of followers/followings to the user.
public class LikeDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string KnownAs { get; set; }
    public string PhotoUrl { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
}