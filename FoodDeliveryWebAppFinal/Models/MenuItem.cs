using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebAppFinal.Models;

public class MenuItem
{
    [Key]
    public int MenuItemID { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string PhotoUrl { get; set; } 

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public int RestaurantID { get; set; }

    [ForeignKey("RestaurantID")]
    public Restaurant Restaurant { get; set; }

    [Required]
    public int CategoryID { get; set; } 

    [ForeignKey("CategoryID")]
    public Category Category { get; set; } 
}