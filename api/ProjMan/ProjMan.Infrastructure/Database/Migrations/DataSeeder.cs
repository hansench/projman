using ProjMan.Application.Security;
using ProjMan.Infrastructure.Database.Entities;

namespace ProjMan.Infrastructure.Database.Migrations;

public static class DataSeeder
{
    public static async Task SeedTestUser(ProjManDbContext context, IPasswordHasher passwordHasher)
    {
        // ensure at least 1 user exists
        if (!context.AppUserDbSet.Any())
        {
            var salt = Guid.NewGuid().ToByteArray();
            var user = new AppUserEntity
            {
                UserName = "admin@mymail.com",
                FullName = "Admin",
                RoleId = 1,
                Hashed = passwordHasher.Hash("qwertyX@123", salt),
                Salted = salt,
                InsertedUserId = 0,
            };

            await context.AppUserDbSet.AddAsync(user);

            await context.SaveChangesAsync();
        }
    }
}
