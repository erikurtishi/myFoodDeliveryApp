using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebAppFinal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminRepository _adminRepository;

    public AdminController(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var users = await _adminRepository.GetAllUsersAsync();
        return View(users); // Return a view to list users
    }
    // GET: CreateDriver
    [HttpGet]
    public IActionResult CreateDriver()
    {
        return View();
    }
    // POST: CreateDriver
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDriver(CreateDriverViewModel model)
    {
        if (ModelState.IsValid)
        {
            var success = await _adminRepository.CreateDriverAsync(model);
            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create driver.");
        }

        return View(model);
    }
    // GET: CreateRestaurant
    [HttpGet]
    public IActionResult CreateRestaurant()
    {
        return View();
    }
    // POST: CreateRestaurant
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRestaurant(CreateRestaurantViewModel model)
    {
        if (ModelState.IsValid)
        {
            var success = await _adminRepository.CreateRestaurantAsync(model);
            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create restaurant.");
        }

        return View(model);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await _adminRepository.DeleteUserAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "Error occurred while deleting the user.";
        return RedirectToAction("Index");
    }
}