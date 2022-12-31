namespace API.Extentions;

public static class IdentityServiceExtentions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration configuration)
    {
        // configure the identities tables and services
        Services.AddIdentityCore<ApplicationUser>(options =>
        {
            // If I wanna change some configurations (ex: password, login, ....)
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<ApplicationRole>()
        .AddRoleManager<RoleManager<ApplicationRole>>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddRoleValidator<RoleValidator<ApplicationRole>>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        #region Jwt and policy auth

        Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["tokenKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        Services.AddAuthorization(options =>
        {
            //               policy name as I want             the role/s that policy permission it
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
        });

        #endregion

        return Services;
    }
}