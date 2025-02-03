using System.Diagnostics;
using FoodDeliveryWebAppFinal.Data;
using Microsoft.AspNetCore.Mvc;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        TempData["Notification"] = TempData["Notification"]; 

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

        var menuItem = await _context.MenuItems.FindAsync(menuItemId);
        if (menuItem == null) return NotFound();

        var activeOrder = await _orderRepository.GetActiveOrderForUserAsync(user.Id);

        if (activeOrder != null && activeOrder.RestaurantID != menuItem.RestaurantID)
        {
            TempData["Notification"] = "You can only order from one restaurant at a time!";
            return RedirectToAction("Index");
        }

        if (activeOrder == null)
        {
            activeOrder = new Order
            {
                UserID = user.Id,
                RestaurantID = menuItem.RestaurantID,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };

            _context.Orders.Add(activeOrder);
            await _context.SaveChangesAsync();
        }

        var existingOrderItem = await _context.OrderItems.FirstOrDefaultAsync(oi =>
            oi.OrderID == activeOrder.OrderID && oi.MenuItemID == menuItemId);

        if (existingOrderItem != null)
        {
            existingOrderItem.Quantity++;
            existingOrderItem.SubTotal += menuItem.Price;
        }
        else
        {
            var orderItem = new OrderItem
            {
                OrderID = activeOrder.OrderID,
                MenuItemID = menuItemId,
                Quantity = 1,
                SubTotal = menuItem.Price
            };

            _context.OrderItems.Add(orderItem);
        }

        activeOrder.TotalAmount = await _context.OrderItems
            .Where(oi => oi.OrderID == activeOrder.OrderID)
            .SumAsync(oi => oi.SubTotal);

        activeOrder.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Notification"] = "Item added to your order!";
        return RedirectToAction("Checkout");
    }
    
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var order = await _orderRepository.GetActiveOrderForUserAsync(user.Id);
        if (order == null || order.OrderItems == null || !order.OrderItems.Any())
        {
            TempData["Notification"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.OrderID);
        var totalAmount = orderItems.Sum(oi => oi.SubTotal);
        ViewData["OrderTotal"] = totalAmount;

        return View(orderItems);
    }
    
    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> UpdateOrderItem(int orderItemId, int quantity)
    {
        var orderItem = await _context.OrderItems.FindAsync(orderItemId);
        if (orderItem == null) return NotFound();

        var menuItem = await _context.MenuItems.FindAsync(orderItem.MenuItemID);
        if (menuItem == null) return NotFound();

        orderItem.Quantity = quantity;
        orderItem.SubTotal = quantity * menuItem.Price;

        await _orderRepository.UpdateOrderItemAsync(orderItem);

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderID == orderItem.OrderID);

        if (order != null)
        {
            order.TotalAmount = order.OrderItems.Sum(oi => oi.SubTotal);
            order.UpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Checkout");
    }
    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> RemoveOrderItem(int orderItemId)
    {
        await _orderRepository.RemoveOrderItemAsync(orderItemId);
        return RedirectToAction("Checkout");
    }

    
    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> CompleteCheckout(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null || order.Status != "Pending") return NotFound();

        order.TotalAmount = order.OrderItems.Sum(oi => oi.SubTotal);

        var user = await _context.Users.FindAsync(order.UserID);
        if (user == null)
        {
            TempData["Notification"] = "User not found.";
            return RedirectToAction("Index");
        }

        var restaurant = await _context.Restaurants
            .Include(r => r.User) 
            .FirstOrDefaultAsync(r => r.RestaurantID == order.RestaurantID);

        if (restaurant == null || restaurant.User == null)
        {
            TempData["Notification"] = "Restaurant or its owner not found.";
            return RedirectToAction("Index");
        }

        var restaurantOwner = restaurant.User;

        if (user.Balance < order.TotalAmount)
        {
            TempData["Notification"] = "Insufficient funds. Please add more balance to complete the order.";
            return RedirectToAction("Balance");
        }

        user.Balance -= order.TotalAmount; 
        restaurantOwner.Balance += order.TotalAmount; 

        order.Status = "Ordered";
        order.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user); 
        _context.Users.Update(restaurantOwner); 
        _context.Orders.Update(order); 

        await _context.SaveChangesAsync();

        TempData["Notification"] = "Your order has been placed successfully!";
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> OrderHistory()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var orders = await _orderRepository.GetOrdersByUserAsync(user.Id);
        return View(orders);
    }
    public async Task<IActionResult> MenuItemDetails(int id)
    {
        var menuItem = await _context.MenuItems
            .Include(m => m.Category)
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.MenuItemID == id);

        if (menuItem == null)
        {
            return NotFound("MenuItem not found.");
        }

        return View(menuItem);
    }
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> Balance()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        return View(user); // Pass the user object to view their balance
    }

    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            TempData["Error"] = "Invalid deposit amount.";
            return RedirectToAction("Balance");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        user.Balance += amount; 
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Deposit successful!";
        return RedirectToAction("Balance");
    }
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            TempData["Notification"] = "Please enter a search term.";
            return RedirectToAction("Index");
        }

        // Search for restaurants whose names contain the query (case-insensitive)
        var restaurants = await _context.Restaurants
            .Where(r => EF.Functions.Like(r.Name, $"%{query}%"))
            .ToListAsync();

        // Search for menu items whose names contain the query
        var menuItems = await _context.MenuItems
            .Where(m => EF.Functions.Like(m.Name, $"%{query}%"))
            .Include(m => m.Category)
            .Include(m => m.Restaurant)
            .ToListAsync();

        var viewModel = new SearchViewModel
        {
            Query = query,
            Restaurants = restaurants,
            MenuItems = menuItems
        };

        return View(viewModel);
    }
}

