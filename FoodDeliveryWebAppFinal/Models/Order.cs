using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebAppFinal.Models;

public class Order
{
    [Key]
    public int OrderID { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending";

    [Required]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string UserID { get; set; }

    [ForeignKey("UserID")]
    public AppUser User { get; set; }
    
    public int? DriverID { get; set; }

    [ForeignKey("DriverID")]
    public Driver? Driver { get; set; }
    
    [Required]
    public int RestaurantID { get; set; }

    [ForeignKey("RestaurantID")]
    public Restaurant Restaurant { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}