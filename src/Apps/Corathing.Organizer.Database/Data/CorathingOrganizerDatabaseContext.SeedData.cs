using System;
using System.Data;

using Corathing.Organizer.Database.Model.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Corathing.Organizer.Database.Data;

public partial class CorathingOrganizerDatabaseContext
{
    public static async Task SeedDataAsync(
        CorathingOrganizerDatabaseContext context,
        UserManager<IdentityUserEntity> userManager,
        RoleManager<IdentityRole<string>> roleManager)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(roleManager);

        if (await context.Roles.AnyAsync() || await context.Users.AnyAsync())
        {
            Log.Information("Roles or Users already exist. Skipping seeding default data.");
            return;
        }

        // 기본 Role 생성
        List<IdentityRole<string>> roles = new();
        List<string> roleStrings = new() { "Admin", "Editor", "User", "Viewer" };

        foreach (string roleString in roleStrings)
        {
            if (!await roleManager.RoleExistsAsync(roleString))
            {
                var role = new IdentityRole<string>
                {
                    Id = roleString,
                    Name = roleString,
                    NormalizedName = roleString.ToUpperInvariant(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                IdentityResult roleResult = await roleManager.CreateAsync(role);
                if (roleResult.Succeeded)
                {
                    Log.Debug("Created default role: {Role}", roleString);
                    roles.Add(role);
                }
            }
        }

        // 기본 User 생성
        var admin = new IdentityUserEntity()
        {
            UserName = "admin",
            Email = "admin@corathing.com",
            Name = "Administrator",
            EmailConfirmed = true,
            IsActive = true,
            PasswordUpdatedAt = DateTime.UtcNow,
        };

        IdentityResult identityResult = await userManager.CreateAsync(admin, "admin");
        if (identityResult.Succeeded)
        {
            Log.Debug("Created default admin user");
            await userManager.AddToRolesAsync(admin, roleStrings);
        }
        else
        {
            Log.Error("Failed to create default admin user: {Errors}", string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            throw new DataException("Failed to create default admin user");
        }
    }

}
