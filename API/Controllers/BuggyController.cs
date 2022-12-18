namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly ApplicationDbContext _dbContext;

    public BuggyController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "Secret Key";
    }

    [HttpGet("not-found")]
    public ActionResult<ApplicationUser> GetNotFound()
    {
        var thing = _dbContext.Users.Find(-1);

        if (thing is null) return NotFound();

        return Ok(thing);
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _dbContext.Users.Find(-1);
        // thing will be null here.
        var thingToReturn = thing.ToString();
        // when you wanna apply method like 'ToString()' to a null this will produce an internal server error

        return thingToReturn;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }
}