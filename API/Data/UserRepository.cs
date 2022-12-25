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
        var query = _dbContext.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
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
