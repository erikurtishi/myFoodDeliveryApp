using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models;

public class LoginView
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}