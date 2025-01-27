using System.Diagnostics;
using FoodDeliveryWebAppFinal.Data;
using Microsoft.AspNetCore.Mvc;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryWebAppFinal.Controllers;

public class HomeController : Controller
{
    private readonly IHomeRepository _homeRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public HomeController(IHomeRepository homeRepository, IOrderRepository orderRepository, UserManager<AppUser> userManager, AppDbContext context)
    {
        _homeRepository = homeRepository;
        _orderRepository = orderRepository;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _homeRepository.GetAllCategoriesAsync();
        var restaurants = await _homeRepository.GetAllRestaurantsAsync();

        ViewData["Categories"] = categories;
        ViewData["Restaurants"] = restaurants;

        return View();
    }

    public async Task<IActionResult> MenuItemsByCategory(int id)
    {
        var menuItems = await _homeRepository.GetMenuItemsByCategoryAsync(id);
        return View(menuItems);
    }

    public async Task<IActionResult> MenuItemsByRestaurant(int id)
    {
        var menuItems = await _homeRepository.GetMenuItemsByRestaurantAsync(id);
        return View(menuItems);
    }
    
    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> AddToOrder(int menuItemId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var activeOrder = await _orderRepository.GetActiveOrderForUserAsync(user.Id);

        if (activeOrder == null)
        {
            activeOrder = new Order
            {
                UserID = user.Id,
                RestaurantID = (await _context.MenuItems.FindAsync(menuItemId)).RestaurantID,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(activeOrder);
            await _context.SaveChangesAsync();
        }

        var menuItem = await _context.MenuItems.FindAsync(menuItemId);
        if (menuItem == null) return NotFound();

        var orderItem = new OrderItem
        {
            OrderID = activeOrder.OrderID,
            MenuItemID = menuItemId,
            Quantity = 1,
            SubTotal = menuItem.Price
        };

        await _orderRepository.AddMenuItemToOrderAsync(orderItem);

        return RedirectToAction("Checkout");
    }

    // Display Checkout Page
    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var order = await _orderRepository.GetActiveOrderForUserAsync(user.Id);
        if (order == null) return RedirectToAction("Index");

        var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.OrderID);
        ViewData["OrderTotal"] = orderItems.Sum(oi => oi.SubTotal);

        return View(orderItems);
    }

    // Update OrderItem Quantity
    [HttpPost]
    public async Task<IActionResult> UpdateOrderItem(int orderItemId, int quantity)
    {
        var orderItem = await _context.OrderItems.FindAsync(orderItemId);
        if (orderItem == null) return NotFound();

        var menuItem = await _context.MenuItems.FindAsync(orderItem.MenuItemID);
        if (menuItem == null) return NotFound();

        orderItem.Quantity = quantity;
        orderItem.SubTotal = quantity * menuItem.Price;

        await _orderRepository.UpdateOrderItemAsync(orderItem);

        return RedirectToAction("Checkout");
    }

    // Remove OrderItem
    [HttpPost]
    public async Task<IActionResult> RemoveOrderItem(int orderItemId)
    {
        await _orderRepository.RemoveOrderItemAsync(orderItemId);
        return RedirectToAction("Checkout");
    }

    // Complete Checkout
    [HttpPost]
    public async Task<IActionResult> CompleteCheckout(int orderId)
    {
        await _orderRepository.CheckoutAsync(orderId);
        return RedirectToAction("Index");
    }
}