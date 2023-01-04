namespace API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IUserRepository UserRepository => new UserRepository(_dbContext, _mapper);

    public IMessagesRepository MessageRepository => new MessagesRepository(_dbContext, _mapper);

    public ILikesRepository LikesRepository => new LikesRepository(_dbContext);

    public async Task<bool> Complete()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _dbContext.ChangeTracker.HasChanges();
    }
}