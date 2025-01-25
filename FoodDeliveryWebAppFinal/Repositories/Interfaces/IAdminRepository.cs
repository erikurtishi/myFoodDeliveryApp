using FoodDeliveryWebAppFinal.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<bool> CreateDriverAsync(CreateDriverViewModel model);
    Task<bool> CreateRestaurantAsync(CreateRestaurantViewModel model);

    Task<bool> DeleteUserAsync(string userId);
}