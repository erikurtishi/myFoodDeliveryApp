using FoodDeliveryWebAppFinal.Models;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetRestaurantByUserId(string userId);
    Task<List<MenuItem>> GetMenuItemsByRestaurantId(int restaurantId);
    Task<MenuItem?> GetMenuItemById(int id);
    Task<List<Category>> GetAllCategoriesAsync();
    Task AddMenuItem(MenuItem menuItem);
    Task UpdateMenuItem(MenuItem menuItem);
    Task DeleteMenuItem(int id);
}