namespace API.Entities;

// This is like join table
public class UserLike
{
    // the user that is liking the other user ( the user that make a follow 'following')
    public ApplicationUser SourceUser { get; set; }
    public int SourceUserId { get; set; }

    // the user that is the other user following/liking
    public ApplicationUser LikedUser { get; set; }
    public int LikedUserId { get; set; }
}