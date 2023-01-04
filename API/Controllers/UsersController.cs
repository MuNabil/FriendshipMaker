namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    private readonly IUnitOfWork _unitOfWork;
    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _photoService = photoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUsers([FromQuery] UserParams userParams)
    {
        #region Filtering

        var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
        userParams.CurrrentUserName = User.GetUsername();

        // To display the oposite gender for the current user
        if (string.IsNullOrEmpty(userParams.Gender))
            userParams.Gender = gender == "male" ? "female" : "male";

        #endregion

        // users also will contains a pagination information as will as the users themselfs
        var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

        // To add a pagination information to the response headers
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        // and send the users in the response body
        return Ok(users);
    }

    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return Ok(await _unitOfWork.UserRepository.GetMemberByNameAsync(username));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UpdateMemberDto updateMemberDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        _mapper.Map(updateMemberDto, user);

        _unitOfWork.UserRepository.Update(user);
        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user.");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user is null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error is not null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _unitOfWork.Complete())
        {
            // return _mapper.Map<PhotoDto>(photo);
            return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem occured when adding a photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user is null) return NotFound("user not found");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null) return NotFound("photoId not found");
        if (photo.IsMain) return BadRequest("This photo is already the main photo");

        var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
        if (currentMainPhoto is not null) currentMainPhoto.IsMain = false;

        photo.IsMain = true;
        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem occured when setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user is null) return NotFound();

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("You cann't remove your main photo");

        if (photo.PublicId is not null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error is not null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);
        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem occured when deleting the photo");
    }
}
