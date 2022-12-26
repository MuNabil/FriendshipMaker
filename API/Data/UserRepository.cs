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

    public async Task<MemberDto> GetMemberByNameAsync(string username)
    {
        return await _dbContext.Users
            .Where(u => u.UserName.Equals(username))
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        // Get the Birth date of min and max age of users to filter by
        // I can't use userParams.MinAge because it's an int and DateOfBirth that I wanna filter depend on is a DateTime
        // So I must declaring a new variables for them
        var minAge = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxAge = DateTime.Today.AddYears(-userParams.MinAge);

        var query = _dbContext.Users
                    .Where(u => u.UserName != userParams.CurrrentUserName && u.Gender == userParams.Gender
                    && u.DateOfBirth >= minAge && u.DateOfBirth <= maxAge) // filtering (note that you must make filtering before ProjectTo())
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking(); // To turn off tracking of EF because we only will read here 'no adding, edit..'

        // To execute the query and returning pageList of ( memberDto and other paging informations that in the pageList class as properities).
        return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<ApplicationUser> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(p => p.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public void Update(ApplicationUser user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
    }
}
