namespace API.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser> GetUserByIdAsync(int id);
    Task<ApplicationUser> GetUserByUsernameAsync(string username);
    Task<MemberDto> GetMemberByNameAsync(string username, bool isCurrenyUser);
    Task<IEnumerable<ApplicationUser>> GetUsersAsync();
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    void Update(ApplicationUser user);
    Task<string> GetUserGender(string username);
    Task<ApplicationUser> GetUserByPhotoId(int photoId);
}
