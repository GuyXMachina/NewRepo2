using GuitarShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Gym" },
                    new Category { CategoryName = "Laundry Service" },
                    new Category { CategoryName = "Parking Service" },
                    new Category { CategoryName = "Library Discussion Room" }
                );
                context.SaveChanges();  
            }

            if (!context.Facilities.Any())
            {
                context.Facilities.AddRange(
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Gym").CategoryID,
                        Code = "Gym_QQ",
                        Name = "Main Gym",
                        Price = (decimal)10.00,
                        PricingType = PricingType.Monthly
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Laundry Service").CategoryID,
                        Code = "laundry_QQ",
                        Name = "Laundry Room 1",
                        Price = (decimal)5.00,
                        PricingType = PricingType.Weekly
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Parking Service").CategoryID,
                        Code = "Parking_QQ",
                        Name = "Parking Lot A",
                        Price = (decimal)3.00,
                        PricingType = PricingType.Daily
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Library Discussion Room").CategoryID,
                        Code = "library_QQ",
                        Name = "Library Discussion Room 1",
                        Price = (decimal)0.00,
                        PricingType = PricingType.Daily
                    }
                );
                context.SaveChanges();  
            }
        }


        public static async void CreateRolesAndUsers(IApplicationBuilder app)
        {
            string[] roles = new string[] { "FacilityAdmin", "FacilityManager", "FacilityInCharge", "User" };
            UserManager<User> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<User>>();

            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var user = new User
            {
                UserName = "FacilityAdmin",
                Email = "Admin@example.com",
                UserType = "Admin" 
            };
            var result = await userManager.CreateAsync(user, "Password123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "FacilityAdmin");
            }
        }


    }
}
