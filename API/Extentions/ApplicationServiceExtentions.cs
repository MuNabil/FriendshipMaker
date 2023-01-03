namespace API.Extentions;

public static class ApplicationServiceExtentions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration configuration)
    {
        Services.AddScoped<ITokenService, TokenService>();

        Services.AddScoped<IUserRepository, UserRepository>();

        Services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        Services.AddScoped<IPhotoService, PhotoService>();

        Services.AddScoped<ILikesRepository, LikesRepository>();

        Services.AddScoped<IMessagesRepository, MessagesRepository>();

        Services.AddScoped<LogUserActivity>();

        // Because I need this service to be shared amongs every single connection that comes into the server
        Services.AddSingleton<PresenceTracker>();

        Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        return Services;
    }
}