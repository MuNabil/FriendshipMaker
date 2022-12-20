namespace API.Extentions;

public static class ApplicationServiceExtentions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration configuration)
    {
        Services.AddScoped<ITokenService, TokenService>();

        Services.AddScoped<IUserRepository, UserRepository>();

        Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        return Services;
    }
}