using System.Security.Claims;
using FoodDeliveryWebAppFinal.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<IdentityResult> RegisterAsync(AppUser user, string password);
    Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
    Task<AppUser?> FindByEmailAsync(string email);
    Task<bool> IsInRoleAsync(AppUser user, string role);
    Task LogoutAsync();
    Task<AppUser?> GetCurrentUserAsync(ClaimsPrincipal user);
    Task<bool> UpdateProfileAsync(ClaimsPrincipal user, EditView model);
}