namespace API.Entities;

public class ApplicationUser : IdentityUser<int>
{
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

    #region Messaging relationship

    // the list of followers to this user
    public ICollection<Message> MessagesSent { get; set; }

    // The list of users that this user following
    public ICollection<Message> MessagesReceived { get; set; }

    #endregion

    // N-M relationship with the roles table
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}
