namespace API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]  // To always change the user lastActive when he doing any action in any controller that derive from this controller.
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{

}
