using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebAppFinal.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly UserManager<AppUser> _userManager;

        public DriverController(IOrderRepository orderRepository,
                                IDriverRepository driverRepository,
                                UserManager<AppUser> userManager)
        {
            _orderRepository = orderRepository;
            _driverRepository = driverRepository;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Dashboard()
        {
            // Retrieve the current user's ID.
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            // Retrieve the driver record using the user's ID.
            var driver = await _driverRepository.GetDriverByUserIdAsync(userId);
            if (driver == null)
            {
                return Unauthorized("Driver record not found for this user.");
            }

            // Get orders assigned to this driver.
            var orders = await _orderRepository.GetOrdersByDriverAsync(driver.DriverID);
            return View(orders);
        }
        
        [HttpPost]
        public async Task<IActionResult> MarkDelivered(int orderId)
        {
            // Retrieve the order.
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Retrieve the current user's ID and get the corresponding driver record.
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var driver = await _driverRepository.GetDriverByUserIdAsync(userId);
            if (driver == null)
            {
                return Unauthorized("Driver record not found for this user.");
            }

            // Ensure the current driver is assigned to this order.
            if (order.DriverID != driver.DriverID)
            {
                return Forbid("You are not authorized to update this order.");
            }

            // Ensure that only orders with the "Out for Delivery" status are updated.
            if (order.Status != "Out for Delivery")
            {
                return BadRequest("Order is not currently out for delivery.");
            }

            // Update the order status.
            order.Status = "Delivered";
            order.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _orderRepository.UpdateOrderAsync(order);
            if (!updateResult)
            {
                return StatusCode(500, "Failed to update order.");
            }

            return RedirectToAction("Dashboard");
        }

        
    }
}
