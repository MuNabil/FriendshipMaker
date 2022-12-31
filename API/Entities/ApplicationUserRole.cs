namespace API.Entities;

//this class is the join table to make N-M relationship between ApplicationUser anf ApplicationRole
public class ApplicationUserRole : IdentityUserRole<int>
{
    public ApplicationUser User { get; set; }
    public ApplicationRole Role { get; set; }
}