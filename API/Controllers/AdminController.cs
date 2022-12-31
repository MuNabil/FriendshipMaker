namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
        .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
        .OrderBy(user => user.UserName)
        .Select(user => new
        {
            user.Id,
            Username = user.UserName,
            Roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToList()
        })
        .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        var newRoles = roles.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();

        var user = await _userManager.FindByNameAsync(username);
        if (user is null) return NotFound("There is no user has this username..!");

        var oldRoles = await _userManager.GetRolesAsync(user);

        // To add the new roles but without add an existing role again
        var result = await _userManager.AddToRolesAsync(user, newRoles.Except(oldRoles));
        if (!result.Succeeded) return BadRequest("Faild to add to roles");

        // To remove the old roles that dosn't exist in the new roles
        result = await _userManager.RemoveFromRolesAsync(user, oldRoles.Except(newRoles));
        if (!result.Succeeded) return BadRequest("Faild to remove roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins and moderators can access this endpoint");
    }
}