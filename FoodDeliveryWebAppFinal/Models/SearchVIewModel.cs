using System.Collections.Generic;

namespace FoodDeliveryWebAppFinal.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<Restaurant> Restaurants { get; set; } = new();
        public List<MenuItem> MenuItems { get; set; } = new();
    }
}