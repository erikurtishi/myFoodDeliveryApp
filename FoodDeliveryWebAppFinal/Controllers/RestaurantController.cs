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
    private readonly UserManager<AppUser> _userManager;

    public RestaurantController(
        IRestaurantRepository restaurantRepository,
        UserManager<AppUser> userManager)
    {
        _restaurantRepository = restaurantRepository;
        _userManager = userManager;
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
}