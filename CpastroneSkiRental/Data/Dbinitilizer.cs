using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Data
{
    /// <summary>
    /// DbInitilizer class is Migrating database and seeding the admin role and creates the admin user if it was no created already.
    /// </summary>
    public class Dbinitilizer
    {
        public static AppSecrets appSecrets { get; set; }
        public static async Task CreateAdminAndUserRoles(IServiceProvider serviceProvider)
        {

            // create the database if it doesn't exist
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();


            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<WebsiteUser>>();
            var newRole = new IdentityRole();

            bool isAdminCreated = await RoleManager.RoleExistsAsync("Admin");
            var isAdminExists = await UserManager.FindByNameAsync("admin@mail.com");

            if (!isAdminCreated)
            {
                await RoleManager.CreateAsync(new IdentityRole("Admin"));
                newRole.Name = "Admin";
                await RoleManager.CreateAsync(newRole);
            }

            if (isAdminExists == null)
            {
                var adminUser = new WebsiteUser
                {
                    UserName = "admin@mail.com",
                    Email = "admin@mail.com",
                    FirstName = "Michael",
                    LastName = "Admin",
                    BirthDate = DateTime.Parse("1990/05/03"),
                    Height = 191,
                    Weight =100,
                    Gender = "M",
                    ShoeSize = 29.5,
                    PantSize = 40,
                    HelmetSize = "M",
                    GloveSize = "M",
                    Level = "Beginner",
                    EmailConfirmed = true
                };

                IdentityResult userCheck = await UserManager.CreateAsync(adminUser, appSecrets.adminPsw);

                if (userCheck.Succeeded)
                {
                    await UserManager.AddToRoleAsync(adminUser, "Admin");
                }

            }

        }
    }
}
