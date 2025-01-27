using FoodDeliveryWebAppFinal.Models;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IHomeRepository
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<List<Restaurant>> GetAllRestaurantsAsync();
    Task<List<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId);
    Task<List<MenuItem>> GetMenuItemsByRestaurantAsync(int restaurantId);
}