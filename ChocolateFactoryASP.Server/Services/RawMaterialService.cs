using ChocolateFactory.Models;
using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class RawMaterialService
    {
        private readonly RawMaterialRepository _repository;
        private readonly WarehouseService _warehouseService;

        public RawMaterialService(RawMaterialRepository repository, WarehouseService warehouseService)
        {
            _repository = repository;
            _warehouseService = warehouseService;
        }

        // Add raw material and update warehouse stock
        public async Task AddRawMaterialAsync(RawMaterial material)
        {
            await _repository.AddRawMaterialAsync(material);
            await UpdateWarehouseStock(material.WarehouseName);
        }

        // Update raw material and warehouse stock
        public async Task UpdateRawMaterialAsync(RawMaterial material)
        {
            await _repository.UpdateRawMaterialAsync(material);
            await UpdateWarehouseStock(material.WarehouseName);
        }

        // Delete raw material and update warehouse stock
        public async Task DeleteRawMaterialAsync(string materialName)
        {
            var material = await _repository.GetRawMaterialByNameAsync(materialName);
            if (material != null)
            {
                await _repository.DeleteRawMaterialAsync(materialName);
                await UpdateWarehouseStock(material.WarehouseName);
            }
        }

        // Delete raw material by batch and update warehouse stock
        public async Task DeleteRawMaterialByBatchAsync(Guid batchId)
        {
            var material = await _repository.GetRawMaterialByBatchIdAsync(batchId);
            if (material != null)
            {
                await _repository.DeleteRawMaterialByBatchAsync(batchId);
                await UpdateWarehouseStock(material.WarehouseName);
            }
        }

        // Update the current stock level of a warehouse
        private async Task UpdateWarehouseStock(string warehouseName)
        {
            // Fetch all raw materials in the warehouse
            var materials = await _repository.GetAllRawMaterialsByWarehouseNameAsync(warehouseName);

            // Initialize stock count
            int totalStock = 0;

            foreach (var material in materials)
            {
                totalStock += ConvertToStockValue(material);
            }

            // Fetch warehouse and update current stock
            var warehouse = await _warehouseService.GetWarehouseByNameAsync(warehouseName);
            if (warehouse != null)
            {
                warehouse.CurrentStockLevel = totalStock;
                await _warehouseService.UpdateWarehouseAsync(warehouse);
            }
        }

        // Helper method: Convert units to stock values
        private int ConvertToStockValue(RawMaterial material)
        {
            return material.Unit switch
            {
                Unit.Kilogram => material.StockQuantity,    // 1 Kilogram -> 1 Stock
                Unit.Liter => material.StockQuantity,       // 1 Liter -> 1 Stock
                Unit.Piece => material.StockQuantity / 10,  // 10 Pieces -> 1 Stock
                Unit.Gram => material.StockQuantity / 1000, // 1000 Grams -> 1 Stock
                Unit.Milliliter => material.StockQuantity / 1000, // 1000 Milliliters -> 1 Stock
                _ => 0
            };
        }

        public async Task<RawMaterial> GetRawMaterialByNameAsync(string materialName)
        {
            return await _repository.GetRawMaterialByNameAsync(materialName);
        }

        public async Task<RawMaterial> GetRawMaterialByBatchIdAsync(Guid materialBatchId)
        {
            return await _repository.GetRawMaterialByBatchIdAsync(materialBatchId);
        }

        public async Task<List<RawMaterial>> GetAllRawMaterialsAsync()
        {
            return await _repository.GetAllRawMaterialsAsync();
        }

        public async Task<IEnumerable<RawMaterial>> GetAllRawMaterialsByWarehouseNameAsync(string warehouseName)
        {
            return await _repository.GetAllRawMaterialsByWarehouseNameAsync(warehouseName);
        }

    }
}
