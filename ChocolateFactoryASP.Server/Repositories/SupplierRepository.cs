using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public class SupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all suppliers
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Set<Supplier>().ToListAsync();
        }

        // Get supplier by ID
        public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
        {
            return await _context.Set<Supplier>().FindAsync(id);
        }

        // Add supplier
        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _context.Set<Supplier>().AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        // Update supplier
        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            _context.Set<Supplier>().Update(supplier);
            await _context.SaveChangesAsync();
        }

        // Delete supplier by ID
        public async Task DeleteSupplierAsync(Guid id)
        {
            var supplier = await GetSupplierByIdAsync(id);
            if (supplier != null)
            {
                _context.Set<Supplier>().Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}
