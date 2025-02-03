using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; 


namespace FoodDeliveryWebAppFinal.Controllers;

[Authorize(Roles = "Restaurant")]
public class RestaurantController : Controller
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDriverRepository _driverRepository;

    public RestaurantController(
        IRestaurantRepository restaurantRepository,
        IOrderRepository orderRepository,
        IDriverRepository driverRepository,
        UserManager<AppUser> userManager)
    {
        _restaurantRepository = restaurantRepository;
        _orderRepository = orderRepository;
        _userManager = userManager;
        _driverRepository = driverRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User is not logged in.");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return Unauthorized("Invalid user.");
        }

        var restaurant = await _restaurantRepository.GetRestaurantByUserId(appUser.Id);
        if (restaurant == null)
        {
            return NotFound("Restaurant not found.");
        }

        return View(restaurant);
    }

    public async Task<IActionResult> MenuItems()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User is not logged in.");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return Unauthorized("Invalid user.");
        }

        var restaurant = await _restaurantRepository.GetRestaurantByUserId(appUser.Id);
        if (restaurant == null)
        {
            return NotFound("Restaurant not found.");
        }

        var menuItems = await _restaurantRepository.GetMenuItemsByRestaurantId(restaurant.RestaurantID);
        ViewData["RestaurantName"] = restaurant.Name;

        return View(menuItems);
    }

    public async Task<IActionResult> CreateMenuItem()
    {
        var categories = await _restaurantRepository.GetAllCategoriesAsync();
        ViewData["Categories"] = categories;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMenuItem(MenuItem model)
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User is not logged in.");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return Unauthorized("Invalid user.");
        }

        var restaurant = await _restaurantRepository.GetRestaurantByUserId(appUser.Id);
        if (restaurant == null)
        {
            return NotFound("Restaurant not found.");
        }

        model.RestaurantID = restaurant.RestaurantID;
        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = DateTime.UtcNow;

        await _restaurantRepository.AddMenuItem(model);

        return RedirectToAction(nameof(MenuItems));
    }

    public async Task<IActionResult> EditMenuItem(int id)
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User is not logged in.");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return Unauthorized("Invalid user.");
        }

        var restaurant = await _restaurantRepository.GetRestaurantByUserId(appUser.Id);
        if (restaurant == null)
        {
            return NotFound("Restaurant not found.");
        }

        var menuItem = await _restaurantRepository.GetMenuItemById(id);
        if (menuItem == null || menuItem.RestaurantID != restaurant.RestaurantID)
        {
            return NotFound("Menu item not found.");
        }

        var categories = await _restaurantRepository.GetAllCategoriesAsync();
        ViewData["Categories"] = categories;

        return View(menuItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMenuItem(MenuItem model)
    {
        var menuItem = await _restaurantRepository.GetMenuItemById(model.MenuItemID);
        if (menuItem == null)
        {
            return NotFound("Menu item not found.");
        }

        menuItem.Name = model.Name;
        menuItem.Description = model.Description;
        menuItem.Price = model.Price;
        menuItem.PhotoUrl = model.PhotoUrl;
        menuItem.IsAvailable = model.IsAvailable;
        menuItem.CategoryID = model.CategoryID;
        menuItem.UpdatedAt = DateTime.UtcNow;

        await _restaurantRepository.UpdateMenuItem(menuItem);

        return RedirectToAction(nameof(MenuItems));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var menuItem = await _restaurantRepository.GetMenuItemById(id);
        if (menuItem == null)
        {
            return NotFound("Menu item not found.");
        }

        await _restaurantRepository.DeleteMenuItem(id);

        return RedirectToAction(nameof(MenuItems));
    }
    
    public async Task<IActionResult> Orders()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User is not logged in.");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return Unauthorized("Invalid user.");
        }

        var restaurant = await _restaurantRepository.GetRestaurantByUserId(appUser.Id);
        if (restaurant == null)
        {
            return NotFound("Restaurant not found.");
        }

        var orders = await _orderRepository.GetOrdersByRestaurantAsync(restaurant.RestaurantID);
        ViewData["RestaurantName"] = restaurant.Name;

        return View(orders);
    }
    public async Task<IActionResult> AssignDriver(int orderId)
    {
        // Retrieve the order (ensure your order repository handles this correctly)
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found.");
        }
        if (order.Status != "Ordered")
        {
            return BadRequest("Order is not in a state to be assigned a driver.");
        }
    
        // Retrieve drivers (ensure the repository returns a non-null list)
        var drivers = await _driverRepository.GetAllDriversAsync() ?? new List<Driver>();
    
        // Build the select list. Here we use the AppUser's UserName as the display text.
        // Adjust this if your AppUser model uses a different property.
        var selectListItems = drivers.Select(d => new SelectListItem
        {
            Value = d.DriverID.ToString(),
            Text = d.User != null ? d.User.UserName : $"Driver {d.DriverID}"
        }).ToList();
    
        var viewModel = new AssignDriverViewModel
        {
            OrderID = orderId,
            DriverSelectList = new SelectList(selectListItems, "Value", "Text")
        };

        return View(viewModel);
    }


    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AssignDriver(AssignDriverViewModel model)
{
    if (!ModelState.IsValid)
    {
        var drivers = await _driverRepository.GetAllDriversAsync() ?? new List<Driver>();
        var selectListItems = drivers.Select(d => new SelectListItem
        {
            Value = d.DriverID.ToString(),
            Text = d.User != null ? d.User.UserName : $"Driver {d.DriverID}"
        }).ToList();
        model.DriverSelectList = new SelectList(selectListItems, "Value", "Text");
        return View(model);
    }

    // Retrieve the order
    var order = await _orderRepository.GetOrderByIdAsync(model.OrderID);
    if (order == null)
    {
        return NotFound("Order not found.");
    }
    if (order.Status != "Ordered")
    {
        return BadRequest("Order is not in a state that can be assigned a driver.");
    }

    // Check if the selected driver exists
    var driver = await _driverRepository.GetDriverByIdAsync(model.SelectedDriverId);
    if (driver == null)
    {
        ModelState.AddModelError("", "Selected driver does not exist.");
        var drivers = await _driverRepository.GetAllDriversAsync() ?? new List<Driver>();
        var selectListItems = drivers.Select(d => new SelectListItem
        {
            Value = d.DriverID.ToString(),
            Text = d.User != null ? d.User.UserName : $"Driver {d.DriverID}"
        }).ToList();
        model.DriverSelectList = new SelectList(selectListItems, "Value", "Text");
        return View(model);
    }

    // Update the order with the selected driver
    order.DriverID = driver.DriverID;
    order.Status = "Out for Delivery";
    order.UpdatedAt = DateTime.UtcNow;

    var updateResult = await _orderRepository.UpdateOrderAsync(order);
    if (!updateResult)
    {
        ModelState.AddModelError("", "Failed to update order.");
        var drivers = await _driverRepository.GetAllDriversAsync() ?? new List<Driver>();
        var selectListItems = drivers.Select(d => new SelectListItem
        {
            Value = d.DriverID.ToString(),
            Text = d.User != null ? d.User.UserName : $"Driver {d.DriverID}"
        }).ToList();
        model.DriverSelectList = new SelectList(selectListItems, "Value", "Text");
        return View(model);
    }

    return RedirectToAction(nameof(Orders));
}




    }

