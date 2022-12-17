namespace API.Controllers;

public class UsersController : BaseApiController
{
    public ApplicationDbContext _dbContext { get; }
    public UsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers() =>
         await _dbContext.Users.ToListAsync();

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUser>> GetUserById(int id) =>
         await _dbContext.Users.FindAsync(id);
}
