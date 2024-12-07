using ChocolateFactory.Data;
using ChocolateFactory.Models;
using ChocolateFactory.Requests;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IWarehouseRepository
    {
        Task<Warehouse> GetWarehouseByNameAsync(string name);
        Task<List<Warehouse>> GetAllWarehousesAsync();
        Task AddWarehouseAsync(Warehouse warehouse);
        Task UpdateWarehouseAsync(Warehouse warehouse);
        Task DeleteWarehouseAsync(string warehouse);
    }

    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse> GetWarehouseByNameAsync(string name)
        {
            return await _context.Warehouses.FindAsync(name);
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync() =>
            await _context.Warehouses.ToListAsync();

        public async Task AddWarehouseAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWarehouseAsync(string warehouse)
        {
            var temp = await GetWarehouseByNameAsync(warehouse);
            if (temp != null)
            {
                _context.Warehouses.Remove(temp);
                await _context.SaveChangesAsync();
            }
        }
    }

}
