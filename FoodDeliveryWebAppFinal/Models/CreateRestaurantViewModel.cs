using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models;

public class CreateRestaurantViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [Display(Name = "Full Name")] 
    public string FullName { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    [Display(Name = "Phone Number")] 
    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; }

    // Restaurant fields
    [Required(ErrorMessage = "Restaurant Name is required.")]
    [Display(Name = "Restaurant Name")] 
    public string RestaurantName { get; set; }
    
    [Display(Name = "Description")] 
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Operating Hours are required.")]
    [Display(Name = "Operating Hours")] 
    public string OperatingHours { get; set; }
    
    [Required(ErrorMessage = "Image is required.")]
    [Display(Name = "Image URL")] 
    public string PhotoUrl { get; set; }
}