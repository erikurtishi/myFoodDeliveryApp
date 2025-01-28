using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly AppDbContext _context;

    public RestaurantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetRestaurantByUserId(string userId)
    {
        return await _context.Restaurants.FirstOrDefaultAsync(r => r.UserID == userId);
    }

    public async Task<List<MenuItem>> GetMenuItemsByRestaurantId(int restaurantId)
    {
        return await _context.MenuItems
            .Where(mi => mi.RestaurantID == restaurantId)
            .Include(mi => mi.Category)
            .ToListAsync();
    }

    public async Task<MenuItem?> GetMenuItemById(int id)
    {
        return await _context.MenuItems
            .Include(mi => mi.Category)
            .FirstOrDefaultAsync(mi => mi.MenuItemID == id);
    }
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync() ?? new List<Category>();
    }

    public async Task AddMenuItem(MenuItem menuItem)
    {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMenuItem(MenuItem menuItem)
    {
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenuItem(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem != null)
        {
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
        }
    }
}