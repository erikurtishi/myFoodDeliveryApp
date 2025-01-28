using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations;

public class HomeRepository : IHomeRepository
{
    private readonly AppDbContext _context;

    public HomeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<List<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _context.Restaurants.ToListAsync();
    }

    public async Task<List<MenuItem>> GetMenuItemsByCategoryAsync(int categoryId)
    {
        return await _context.MenuItems
            .Include(mi => mi.Restaurant)
            .Include(mi => mi.Category)
            .Where(mi => mi.CategoryID == categoryId)
            .ToListAsync();
    }

    public async Task<List<MenuItem>> GetMenuItemsByRestaurantAsync(int restaurantId)
    {
        return await _context.MenuItems
            .Include(mi => mi.Restaurant)
            .Include(mi => mi.Category)
            .Where(mi => mi.RestaurantID == restaurantId)
            .ToListAsync();
    }
}