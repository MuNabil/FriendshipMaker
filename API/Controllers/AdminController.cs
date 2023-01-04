namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;
    public AdminController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
    {
        _photoService = photoService;
        _unitOfWork = unitOfWork;
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
    public async Task<ActionResult> GetPhotosForModeration()
    {
        return Ok(await _unitOfWork.PhotoRepository.GetUnapprovedPhotos());
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo is null) return NotFound();

        photo.IsApproved = true;

        var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem occured when approving the photo with ID = " + photoId);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo is null) return NotFound();

        if (photo.PublicId is not null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Result == "ok") _unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        else
        {
            _unitOfWork.PhotoRepository.RemovePhoto(photo);
        }

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem occured when deleting the photo");
    }
}