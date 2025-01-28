using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebAppFinal.Models;

public class Driver
{
    [Key]
    public int DriverID { get; set; }
    public string VehicleType { get; set; } 
    public string LicenseNumber { get; set; }
    [Required]
    public string UserID { get; set; }
    [ForeignKey("UserID")]
    
    public AppUser User { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}