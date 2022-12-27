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

    public async Task<IEnumerable<LikeDto>> GetUserLikesAsync(string predicate, int userId)
    {
        // The only reason I get the users because If the pridicate is null or invalid, it will return all the users
        var users = _dbContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dbContext.Likes.AsQueryable();

        if (predicate == "liked")
        { // liked = following

            // SourceUser that following the other user
            likes = likes.Where(like => like.SourceUserId == userId);

            // Get his followings users
            users = likes.Select(like => like.LikedUser);
        }

        if (predicate == "likedBy")
        { // likedBy = follower

            // LikedUser is the user that is the SourceUser following then the SourceUser is the followers to the LikedUser
            likes = likes.Where(like => like.LikedUserId == userId);

            //Get his followers
            users = likes.Select(like => like.SourceUser);
        }

        return await users.Select(user => new LikeDto
        {  // To do mapping
            Id = user.Id,
            Username = user.UserName,
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            Age = user.DateOfBirth.CalculateAge(),
            City = user.City
        }).ToListAsync();
    }

    public async Task<ApplicationUser> GetUserWithLikesAsync(int userId)
    {
        return await _dbContext.Users.Include(u => u.LikedUsers).FirstOrDefaultAsync(u => u.Id == userId);
    }
}