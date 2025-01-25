using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AdminRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<bool> CreateDriverAsync(CreateDriverViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                IsAvailable = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Driver");

                var driver = new Driver
                {
                    UserID = user.Id,
                    VehicleType = model.VehicleType,
                    LicenseNumber = model.LicenseNumber
                };

                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> CreateRestaurantAsync(CreateRestaurantViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                IsAvailable = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Restaurant");

                var restaurant = new Restaurant
                {
                    UserID = user.Id,
                    Name = model.RestaurantName,
                    Description = model.Description,
                    OperatingHours = model.OperatingHours,
                    PhotoUrl = model.PhotoUrl
                };

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
}