namespace API.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser> GetUserByIdAsync(int id);
    Task<ApplicationUser> GetUserByUsernameAsync(string username);
    Task<MemberDto> GetMemberByNameAsync(string username);
    Task<IEnumerable<ApplicationUser>> GetUsersAsync();
    Task<IEnumerable<MemberDto>> GetMembersAsync();
    void Update(ApplicationUser user);
    Task<bool> SaveAllAsync();
}
