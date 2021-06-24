using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Storage.API.Core;

namespace Storage.API
{
    public static class Seed
    {
        public static async Task AddData(IServiceProvider serviceProvider)
        {
            var data = serviceProvider.GetService<IOptions<SeedData>>()?.Value;

            if (data?.Users?.Any() ?? false)
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = data.Users.Where(x => x.Role != null).Select(x => x.Role).Distinct();
                foreach (var role in roles)
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                foreach (var seedUser in data.Users)
                {
                    var user = await userManager.FindByNameAsync(seedUser.UserName);
                    if (user == null)
                    {
                        user = new ApplicationUser()
                        {
                            UserName = seedUser.UserName,
                        };
                        var userCreateResult = await userManager.CreateAsync(user, seedUser.Password);
                        if (userCreateResult.Succeeded && seedUser.Role != null)
                            await userManager.AddToRoleAsync(user, seedUser.Role);
                    }
                    else if (seedUser.Role != null && !await userManager.IsInRoleAsync(user, seedUser.Role))
                    {
                        var userRoles = await userManager.GetRolesAsync(user);
                        await userManager.RemoveFromRolesAsync(user, userRoles);
                        await userManager.AddToRoleAsync(user, seedUser.Role);
                    }
                }
            }
        }
    }

    public class SeedData
    {
        public List<SeedUser> Users { get; set; }
    }
    
    public class SeedUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}