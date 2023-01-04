namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        // To get the data from this file
        var usersData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
        // to convert the data into actual objects rather than json objects
        var users = JsonSerializer.Deserialize<List<ApplicationUser>>(usersData);

        // Roles
        var roles = new List<ApplicationRole>{
            new ApplicationRole {Name = "Admin"},
            new ApplicationRole {Name = "Moderator"},
            new ApplicationRole {Name = "Member"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        // To seed the users and add them to the Member role
        // Because you add them into the role then you must seed the role first
        foreach (var user in users)
        {
            user.Photos.First().IsApproved = true;

            user.UserName = user.UserName.Trim().ToLower();

            await userManager.CreateAsync(user, "Pa$$w0rd");

            await userManager.AddToRoleAsync(user, "Member");
        }

        // Create a user and add him to Admin and Moderator roles
        var admin = new ApplicationUser { UserName = "admin" };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
    }
}