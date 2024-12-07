using ChocolateFactory.Models;
using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class SupplierService
    {
        private readonly SupplierRepository _repository;

        public SupplierService(SupplierRepository repository)
        {
            _repository = repository;
        }

        // Get all suppliers
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _repository.GetAllSuppliersAsync();
        }

        // Get supplier by ID
        public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
        {
            return await _repository.GetSupplierByIdAsync(id);
        }

        // Add supplier
        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _repository.AddSupplierAsync(supplier);
        }

        // Update supplier
        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _repository.GetSupplierByIdAsync(supplier.SupplierId);
            if (existingSupplier == null)
                throw new Exception("Supplier not found.");

            existingSupplier.Name = supplier.Name;
            existingSupplier.Address = supplier.Address;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Notes = supplier.Notes;

            await _repository.UpdateSupplierAsync(existingSupplier);
        }

        // Delete supplier
        public async Task DeleteSupplierAsync(Guid id)
        {
            await _repository.DeleteSupplierAsync(id);
        }
    }
}
