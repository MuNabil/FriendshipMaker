using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(ApplicationDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync()) return;

        var usersData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<ApplicationUser>>(usersData);

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();

            using var hmac = new HMACSHA512();

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmac.Key;

            dbContext.Users.Add(user);
        }
        await dbContext.SaveChangesAsync();
    }
}