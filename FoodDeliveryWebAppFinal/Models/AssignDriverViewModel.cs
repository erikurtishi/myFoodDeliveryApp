using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebAppFinal.Models
{
    public class AssignDriverViewModel
    {
        [Required]
        public int OrderID { get; set; }
        
        [Required(ErrorMessage = "Please select a driver.")]
        [Display(Name = "Driver")]
        public int SelectedDriverId { get; set; }
        
        // Initialize with an empty SelectList to avoid null references.
        public SelectList DriverSelectList { get; set; } = new SelectList(new List<SelectListItem>());
    }
}