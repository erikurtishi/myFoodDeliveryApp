using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models;

public class Category
{
    [Key]
    public int CategoryID { get; set; } 

    [Required]
    [StringLength(100)]
    public string Name { get; set; } 

    [StringLength(255)]
    public string Description { get; set; } 

    public ICollection<MenuItem> MenuItems { get; set; } 
}