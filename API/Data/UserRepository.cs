namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public UserRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<MemberDto> GetMemberByNameAsync(string username, bool isCurrenyUser)
    {
        var query = _dbContext.Users
            .Where(u => u.UserName.Equals(username))
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        if (isCurrenyUser) query = query.IgnoreQueryFilters();

        return await query.SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {

        var query = _dbContext.Users.AsQueryable();

        #region Filtering

        // Get the Birth date of min and max age of users to filter by
        var minAge = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxAge = DateTime.Today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.UserName != userParams.CurrrentUserName && u.Gender == userParams.Gender
                    && u.DateOfBirth >= minAge && u.DateOfBirth <= maxAge);

        #endregion

        #region Sorting

        query = userParams.OrderBy switch // the new way to do the switch expression
        {
            "created" => query.OrderByDescending(u => u.Created), // equal to case "created":  ...
            _ => query.OrderByDescending(u => u.LastActive)  // (_ => equal to the default case in switch statement)
        };

        #endregion


        // I can't store the result in query variable cuz it's an ApplicationUser type now
        var queryAsMemberDto = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

        // To execute the query and returning pageList of ( memberDto and other paging informations that in the pageList class as properities).
        return await PagedList<MemberDto>.CreateAsync(queryAsMemberDto, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<ApplicationUser> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.Include(p => p.Photos).IgnoreQueryFilters().SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(p => p.Photos).ToListAsync();
    }

    public void Update(ApplicationUser user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _dbContext.Users.Where(u => u.UserName == username).Select(u => u.Gender).FirstOrDefaultAsync();
    }

    public async Task<ApplicationUser> GetUserByPhotoId(int photoId)
    {
        return await _dbContext.Users.Include(u => u.Photos).IgnoreQueryFilters()
        .Where(u => u.Photos.Any(p => p.Id == photoId)).SingleOrDefaultAsync();
    }
}
