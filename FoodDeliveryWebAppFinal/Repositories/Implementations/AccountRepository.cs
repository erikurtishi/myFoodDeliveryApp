using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterAsync(AppUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "BasicUser");
        }
        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    }
    
    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    public async Task<bool> IsInRoleAsync(AppUser user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}