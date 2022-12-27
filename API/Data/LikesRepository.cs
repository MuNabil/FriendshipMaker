namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly ApplicationDbContext _dbContext;
    public LikesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

    }
    public async Task<UserLike> GetUserLikeAsync(int SourceUserId, int LikedUserId)
    {
        // TO get any raw of the likes table by its primary key
        return await _dbContext.Likes.FindAsync(SourceUserId, LikedUserId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams)
    {
        // The only reason I get the users because If the pridicate is null or invalid, it will return all the users
        var users = _dbContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dbContext.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        { // liked = following

            // SourceUser that following the other user
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);

            // Get his followings users
            users = likes.Select(like => like.LikedUser);
        }

        if (likesParams.Predicate == "likedBy")
        { // likedBy = follower

            // LikedUser is the user that is the SourceUser following then the SourceUser is the followers to the LikedUser
            likes = likes.Where(like => like.LikedUserId == likesParams.UserId);

            //Get his followers
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers = users.Select(user => new LikeDto
        {  // To do mapping
            Id = user.Id,
            Username = user.UserName,
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            Age = user.DateOfBirth.CalculateAge(),
            City = user.City
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<ApplicationUser> GetUserWithLikesAsync(int userId)
    {
        return await _dbContext.Users.Include(u => u.LikedUsers).FirstOrDefaultAsync(u => u.Id == userId);
    }
}