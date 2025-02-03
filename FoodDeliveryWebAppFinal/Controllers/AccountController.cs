using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebAppFinal.Controllers;

public class AccountController : Controller
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    // GET: Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterView model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Age = model.Age ?? 0,
                IsAvailable = true
            };

            var result = await _accountRepository.RegisterAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _accountRepository.LoginAsync(model.Email, model.Password, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    // GET: Login
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountRepository.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (result.Succeeded)
            {
                var user = await _accountRepository.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (await _accountRepository.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (await _accountRepository.IsInRoleAsync(user, "Driver"))
                    {
                        return RedirectToAction("Dashboard", "Driver");
                    }
                    else if (await _accountRepository.IsInRoleAsync(user, "Restaurant"))
                    {
                        return RedirectToAction("Index", "Restaurant");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }
    
    // GET: Edit
    [HttpGet]
    [Authorize(Roles = "BasicUser")]
    public async Task<IActionResult> Edit()
    {
        var user = await _accountRepository.GetCurrentUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new EditView
        {
            FullName = user.FullName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            Age = user.Age,
            AgeOptions = Enumerable.Range(18, 83).ToList()
        };

        return View(model);
    }

    // POST: Edit
    [HttpPost]
    [Authorize(Roles = "BasicUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditView model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountRepository.UpdateProfileAsync(User, model);

        if (result)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
        return View(model);
    }
    // POST: Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountRepository.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}