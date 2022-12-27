namespace API.Entities;

public class ApplicationUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string KnownAs { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }

    #region To add the like functionality

    // the list of followers to this user
    public ICollection<UserLike> LikedByUsers { get; set; }

    // The list of users that this user following
    public ICollection<UserLike> LikedUsers { get; set; }

    #endregion
}
