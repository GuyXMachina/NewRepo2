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
                        Picture = "https://www.hussle.com/blog/wp-content/uploads/2020/12/Gym-structure-1080x675.png",
                        Code = "Gym_QQ",
                        Name = "Main Gym",
                        Price = (decimal)100.00,
                        PricingType = PricingType.Monthly
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c=> c.CategoryName == "Laundry Service").CategoryID,
                        Picture = "https://speedqueencommercial.com/en-us/wp-content/uploads/2020/01/Modern-laundry-room-with-Speed-Queen-commercial-laundry-equipment.png",
                        Code = "laundry_QQ",
                        Name = "Laundry Room 1",
                        Price = (decimal)95.00,
                        PricingType = PricingType.Weekly
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Parking Service").CategoryID,
                        Picture = "https://concreteuprising.com/wp-content/uploads/2019/05/iStock-925615406_res.jpg",
                        Code = "Parking_QQ",
                        Name = "Parking Lot A",
                        Price = (decimal)30.00,
                        PricingType = PricingType.Daily
                    },
                    new Facility
                    {
                        CategoryID = context.Categories.First(c => c.CategoryName == "Library Discussion Room").CategoryID,
                        Picture = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/60/Statsbiblioteket_l%C3%A6sesalen-2.jpg/1280px-Statsbiblioteket_l%C3%A6sesalen-2.jpg",
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

            // Admin user
            var adminUser = new User
            {
                UserName = "FacilityAdmin",
                Email = "Admin@example.com",
                UserType = UserType.Staff
            };
            var adminResult = await userManager.CreateAsync(adminUser, "Password123!");

            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "FacilityAdmin");
            }

            // Manager user
            var managerUser = new User
            {
                UserName = "FacilityManager",
                Name = "Admin",
                Surname = "Admin",
                Email = "Manager@example.com",
                UserType = UserType.Staff
            };
            var managerResult = await userManager.CreateAsync(managerUser, "Password123!");

            if (managerResult.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "FacilityManager");
            }

            // In-charge user
            var inChargeUser = new User
            {
                UserName = "FacilityInCharge",
                Email = "InCharge@example.com",
                UserType = UserType.Staff
            };
            var inChargeResult = await userManager.CreateAsync(inChargeUser, "Password123!");

            if (inChargeResult.Succeeded)
            {
                await userManager.AddToRoleAsync(inChargeUser, "FacilityInCharge");
            }

            // Regular user
            var regularUser = new User
            {
                UserName = "RegularUser",
                Email = "User@example.com",
                UserType = UserType.Student
            };
            var userResult = await userManager.CreateAsync(regularUser, "Password123!");

            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }


    }
}
