using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryWebAppFinal.Models;

public class AppUser : IdentityUser
{
    [Required]
    [StringLength(30)]
    public string FullName { get; set; }
    public int? Age { get; set; }
    [Required]
    [StringLength(40)]
    public string Address { get; set; }
    public decimal Balance { get; set; } = 0.00m;
    public bool IsAvailable { get; set; }
}