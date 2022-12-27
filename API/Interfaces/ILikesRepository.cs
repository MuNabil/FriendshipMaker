namespace API.Interfaces;

public interface ILikesRepository
{
    // To get any row in the UserLike table by his primary key
    Task<UserLike> GetUserLikeAsync(int SourceUserId, int LikedUserId);

    // To get any user including his followings
    Task<ApplicationUser> GetUserWithLikesAsync(int userId);

    // To get the user followers/likedHim or following/heLike depend on the pridicate that will send
    Task<IEnumerable<LikeDto>> GetUserLikesAsync(string pridicate, int userId);

}