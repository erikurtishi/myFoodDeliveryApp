using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebAppFinal.Models;

public class OrderItem
{
    [Key]
    public int OrderItemID { get; set; }
    
    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal SubTotal { get; set; }

    [Required]
    public int OrderID { get; set; }

    [ForeignKey("OrderID")]
    public Order Order { get; set; }

    [Required]
    public int MenuItemID { get; set; }

    [ForeignKey("MenuItemID")]
    public MenuItem MenuItem { get; set; }
}