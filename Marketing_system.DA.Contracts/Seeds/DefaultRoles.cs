using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.AspNetCore.Identity;

namespace Marketing_system.DA.Contracts.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRole.Client.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRole.Employee.ToString()));
        }
    }
}
