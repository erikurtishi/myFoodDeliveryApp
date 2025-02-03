using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrdersByUserAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    public async Task<Order?> GetActiveOrderForUserAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.UserID == userId && o.Status == "Pending");
    }

    public async Task<bool> AddMenuItemToOrderAsync(OrderItem orderItem)
    {
        var existingItem = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderID == orderItem.OrderID && oi.MenuItemID == orderItem.MenuItemID);

        if (existingItem != null)
        {
            existingItem.Quantity += orderItem.Quantity;
            existingItem.SubTotal += orderItem.SubTotal;
        }
        else
        {
            _context.OrderItems.Add(orderItem);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveOrderItemAsync(int orderItemId)
    {
        var orderItem = await _context.OrderItems.FindAsync(orderItemId);
        if (orderItem == null) return false;

        _context.OrderItems.Remove(orderItem);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
    {
        return await _context.OrderItems
            .Where(oi => oi.OrderID == orderId)
            .Include(oi => oi.MenuItem)
            .ToListAsync();
    }

    public async Task<bool> CheckoutAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return false;

        order.Status = "Ordered";
        order.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.RestaurantID == restaurantId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

}