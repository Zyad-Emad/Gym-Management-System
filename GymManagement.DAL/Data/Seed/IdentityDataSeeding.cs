using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Seed
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager ,
            UserManager<ApplicationUser> userManager ,
            ILogger logger,
            CancellationToken ct = default)
        {
            try
            {
                bool hasUsers = await userManager.Users.AnyAsync(ct);
                bool hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasRoles && hasUsers) return;
                var roles = new List<IdentityRole>() { new IdentityRole("SuperAdmin"), new IdentityRole("Admin") };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed To Create Role {role.Name} : {string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Zyad",
                        LastName = "Emad",
                        Email = "Zyad.emad19@gmail.com",
                        UserName = "Zoz",
                        PhoneNumber = "01018946895"
                    };
                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Emad",
                        Email = "Mohamed.emad19@gmail.com",
                        UserName = "Mo",
                        PhoneNumber = "01118946895"
                    };
                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    logger.LogInformation($"Identity Data Seeded");

                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity Seeing Failed");
                return;
            }
        }
    }
}
