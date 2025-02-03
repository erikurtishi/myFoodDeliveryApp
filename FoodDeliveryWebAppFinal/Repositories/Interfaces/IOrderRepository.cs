using FoodDeliveryWebAppFinal.Models;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersByUserAsync(string userId);
    Task<Order?> GetActiveOrderForUserAsync(string userId);
    Task<bool> AddMenuItemToOrderAsync(OrderItem orderItem);
    Task<bool> UpdateOrderItemAsync(OrderItem orderItem);
    Task<bool> RemoveOrderItemAsync(int orderItemId);
    Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
    Task<bool> CheckoutAsync(int orderId);
    Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId);
}