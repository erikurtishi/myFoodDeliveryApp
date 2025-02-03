using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebAppFinal.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DeliveryController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<AppUser> _userManager;

        public DeliveryController(IOrderRepository orderRepository, UserManager<AppUser> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve the current delivery person's driver ID.
            // For demonstration, try to get it from a claim; otherwise, use a dummy value.
            var driverIdClaim = User.FindFirst("DriverID");
            int driverId;
            if (driverIdClaim != null && int.TryParse(driverIdClaim.Value, out driverId))
            {
                // Successfully parsed the driver ID from claims.
            }
            else
            {
                // For demonstration purposes, we use a hard-coded driver ID.
                driverId = 1;
            }

            var orders = await _orderRepository.GetOrdersByDriverAsync(driverId);
            return View(orders);
        }
    }
}