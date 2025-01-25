using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models;

public class CreateDriverViewModel
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
    public string? PhoneNumber { get; set; }

    // Driver fields
    [Display(Name = "Vehicle Type")] 
    [Required(ErrorMessage = "Vehicle type is required.")]
    public string VehicleType { get; set; }

    [Display(Name = "License Plate")] 
    [Required(ErrorMessage = "License Plate is required.")]
    public string LicenseNumber { get; set; }
}