using FoodDeliveryWebAppFinal.Data;
using FoodDeliveryWebAppFinal.Models;
using FoodDeliveryWebAppFinal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebAppFinal.Repositories.Implementations
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            // ToListAsync() returns an empty list if there are no records.
            return await _context.Drivers.ToListAsync() ?? new List<Driver>();
        }

        public async Task<Driver?> GetDriverByIdAsync(int driverId)
        {
            return await _context.Drivers.FirstOrDefaultAsync(d => d.DriverID == driverId);
        }
        public async Task<Driver?> GetDriverByUserIdAsync(string userId)
        {
            return await _context.Drivers.Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserID == userId);
        }
    }
}