namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly ApplicationDbContext _dbContext;
    public PhotoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Photo> GetPhotoById(int id)
    {
        return await _dbContext.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await _dbContext.Photos
            .IgnoreQueryFilters().Where(p => p.IsApproved == false)
            .Select(photo => new PhotoForApprovalDto
            {
                Id = photo.Id,
                Url = photo.Url,
                IsApproved = photo.IsApproved,
                Username = photo.ApplicationUser.UserName
            }
            ).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        _dbContext.Photos.Remove(photo);
    }
}