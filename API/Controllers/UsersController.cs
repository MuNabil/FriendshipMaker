namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    public ApplicationDbContext _dbContext { get; }
    public UsersController(ApplicationDbContext dbContext)
    {
            _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers() =>
         await _dbContext.Users.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUser>> GetUserById(int id) =>
         await _dbContext.Users.FindAsync(id);
}
