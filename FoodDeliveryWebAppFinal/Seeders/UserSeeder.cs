using FoodDeliveryWebAppFinal.Models;
using Microsoft.AspNetCore.Identity;
using FoodDeliveryWebAppFinal.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Seeders;

public class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var roles = new[] { "Admin", "BasicUser", "Restaurant", "Driver" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        await CreateUser(userManager, "admin@admin.com", "Admin@123", "Admin", "Admin User", "Admin Address", 35, "1234567890");

        await CreateUser(userManager, "user1@example.com", "User@123", "BasicUser", "User 1", "Basic Address 1", 20, "1111111111");
        await CreateUser(userManager, "user2@example.com", "User@123", "BasicUser", "User 2", "Basic Address 2", 21, "1111111112");
        await CreateUser(userManager, "user3@example.com", "User@123", "BasicUser", "User 3", "Basic Address 3", 22, "1111111113");

        // Create restaurant users and their corresponding Restaurant records
        var restaurant1 = await CreateUser(userManager, "example1@restaurant.com", "Restaurant@123", "Restaurant", "Restaurant 1", "Restaurant Address 1", 30, "2222222221");
        var restaurant2 = await CreateUser(userManager, "example2@restaurant.com", "Restaurant@123", "Restaurant", "Restaurant 2", "Restaurant Address 2", 31, "2222222222");
        var restaurant3 = await CreateUser(userManager, "example3@restaurant.com", "Restaurant@123", "Restaurant", "Restaurant 3", "Restaurant Address 3", 32, "2222222223");

        // Create Restaurant records if they don't exist
        if (restaurant1 != null && !await dbContext.Restaurants.AnyAsync(r => r.UserID == restaurant1.Id))
        {
            dbContext.Restaurants.Add(new Restaurant
            {
                UserID = restaurant1.Id,
                Name = "Restaurant 1",
                Description = "A fine dining restaurant serving international cuisine",
                OperatingHours = "9:00 AM - 10:00 PM",
                PhotoUrl = "https://example.com/restaurant1.jpg",
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        if (restaurant2 != null && !await dbContext.Restaurants.AnyAsync(r => r.UserID == restaurant2.Id))
        {
            dbContext.Restaurants.Add(new Restaurant
            {
                UserID = restaurant2.Id,
                Name = "Restaurant 2",
                Description = "Traditional local cuisine with a modern twist",
                OperatingHours = "10:00 AM - 11:00 PM",
                PhotoUrl = "https://example.com/restaurant2.jpg",
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        if (restaurant3 != null && !await dbContext.Restaurants.AnyAsync(r => r.UserID == restaurant3.Id))
        {
            dbContext.Restaurants.Add(new Restaurant
            {
                UserID = restaurant3.Id,
                Name = "Restaurant 3",
                Description = "Fast casual dining with fresh ingredients",
                OperatingHours = "8:00 AM - 9:00 PM",
                PhotoUrl = "https://example.com/restaurant3.jpg",
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // Create drivers with their corresponding Driver records
        var driver1 = await CreateUser(userManager, "example1@driver.com", "Driver@123", "Driver", "Driver 1", "Driver Address 1", 25, "3333333331");
        var driver2 = await CreateUser(userManager, "example2@driver.com", "Driver@123", "Driver", "Driver 2", "Driver Address 2", 26, "3333333332");
        var driver3 = await CreateUser(userManager, "example3@driver.com", "Driver@123", "Driver", "Driver 3", "Driver Address 3", 27, "3333333333");

        // Create Driver records if they don't exist
        if (driver1 != null && !await dbContext.Drivers.AnyAsync(d => d.UserID == driver1.Id))
        {
            dbContext.Drivers.Add(new Driver { UserID = driver1.Id, VehicleType = "Car", LicenseNumber = "DL001" });
        }
        if (driver2 != null && !await dbContext.Drivers.AnyAsync(d => d.UserID == driver2.Id))
        {
            dbContext.Drivers.Add(new Driver { UserID = driver2.Id, VehicleType = "Motorcycle", LicenseNumber = "DL002" });
        }
        if (driver3 != null && !await dbContext.Drivers.AnyAsync(d => d.UserID == driver3.Id))
        {
            dbContext.Drivers.Add(new Driver { UserID = driver3.Id, VehicleType = "Car", LicenseNumber = "DL003" });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task<AppUser> CreateUser(UserManager<AppUser> userManager, string email, string password, string role, string fullName, string address, int age, string phoneNumber)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Address = address,
                Age = age,
                PhoneNumber = phoneNumber,
                IsAvailable = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                return user;
            }
        }
        return await userManager.FindByEmailAsync(email);
    }
}