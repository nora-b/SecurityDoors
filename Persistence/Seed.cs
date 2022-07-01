using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
           
            if (!roleManager.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role{Name = "Admin"},
                    new Role{Name = "Employee"},
                    new Role{Name = "Guest"},
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                    await context.SaveChangesAsync();
                }
            } 

            if (!userManager.Users.Any())
            {
                //adding the admin user for the first time when db is created
                var user = new User
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = "admin_admin",
                    InOffice = false
                };

                await userManager.CreateAsync(user, "Pa$$word00!");
                await context.SaveChangesAsync();
                //now we have to add the role for this user
                var roleNameAdmin = await roleManager.FindByNameAsync("Admin");
                await userManager.AddToRoleAsync(user, roleNameAdmin.Name);

                user.Tag = new Tag
                {
                    Code = $"{user.Id}-{Guid.NewGuid()}",
                    UserId = user.Id,
                    StatusTunnel = TagStatus.Active,
                    IsAuthorizedTunnel = true,
                    StatusOffice = TagStatus.Active,
                    IsAuthorizedOffice = true,
                    TagTunnelExpiresAt = DateTimeOffset.UtcNow.AddYears(3),
                    TagOfficeExpiresAt = DateTimeOffset.UtcNow.AddYears(3)
                };

                await context.SaveChangesAsync();

            }
        }

    }
        
}