using FoodDeliveryWebAppFinal.Models;

namespace FoodDeliveryWebAppFinal.Repositories.Interfaces;

public interface IDriverRepository
{
    Task<List<Driver>> GetAllDriversAsync();
    Task<Driver?> GetDriverByIdAsync(int driverId);
}