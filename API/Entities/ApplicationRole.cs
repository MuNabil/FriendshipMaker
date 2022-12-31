namespace API.Entities;

// This class just to make an N-M relationship with the applicationUser
public class ApplicationRole : IdentityRole<int>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}