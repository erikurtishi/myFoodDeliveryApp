using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models;

public class EditView
{
    [Required]
    [StringLength(50)]
    public string FullName { get; set; }

    [Required]
    [StringLength(100)]
    public string Address { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }

    public int? Age { get; set; }
    public List<int> AgeOptions { get; set; } = Enumerable.Range(18, 83).ToList(); 

}