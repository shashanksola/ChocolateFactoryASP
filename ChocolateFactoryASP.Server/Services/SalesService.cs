using ChocolateFactory.Models;
using ChocolateFactory.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using ChocolateFactory.Data;

namespace ChocolateFactory.Services
{
    public class SalesService
    {
        private readonly SalesOrderRepository _repository;
        private readonly WarehouseRepository _warehouseRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly FinishedGoodsRepository _finishedGoodsRepository;

        public SalesService(SalesOrderRepository repository, WarehouseRepository warehouseRepository, ApplicationDbContext dbContext, FinishedGoodsRepository finishedGoodsRepository)
        {
            _repository = repository;
            _warehouseRepository = warehouseRepository;
            _applicationDbContext = dbContext;
            _finishedGoodsRepository = finishedGoodsRepository;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllOrdersAsync()
        {
            return await _repository.GetAllOrdersAsync();
        }

        public async Task AddOrderAsync(SalesOrder order)
        {
            await _repository.AddOrderAsync(order);
        }

        public async Task DeductWarehouseStockAsync(Guid productId, int quantity)
        {
            FinishedGood good = await _finishedGoodsRepository.GetFinishedGoodByIdAsync(productId);
            string warehouse = good.WarehouseLocation;

            Warehouse warehouseItem = await _warehouseRepository.GetWarehouseByNameAsync(warehouse);
            if (warehouseItem == null || warehouseItem.CurrentStockLevel < quantity)
                throw new Exception("Insufficient stock in the warehouse");

            warehouseItem.CurrentStockLevel -= quantity;
            await _warehouseRepository.UpdateWarehouseAsync(warehouseItem);
        }

        public async Task DeductFinishedGoodStockAsync(Guid productId, int quantity)
        {
            var product = await _finishedGoodsRepository.GetFinishedGoodByIdAsync(productId);
            if (product == null || product.Quantity < quantity)
                throw new Exception("Insufficient stock in finished goods");

            product.Quantity -= quantity;
            await _finishedGoodsRepository.UpdateFinishedGoodAsync(product);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _applicationDbContext.Database.BeginTransactionAsync();
        }

    }
}
