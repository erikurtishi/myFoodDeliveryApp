using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebAppFinal.Models;

public class Restaurant
{
    [Key]
    public int RestaurantID { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public string OperatingHours { get; set; } 

    public string PhotoUrl { get; set; } 

    public decimal Balance { get; set; } = 0.00m; 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserID { get; set; } 

    [ForeignKey("UserID")]
    public AppUser User { get; set; }

    // public ICollection<MenuItem> MenuItems { get; set; }
    //
    // // Add this property for the relationship with orders
    // public ICollection<Order> Orders { get; set; } = new List<Order>();
}