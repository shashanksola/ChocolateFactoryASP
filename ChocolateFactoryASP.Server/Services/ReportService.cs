using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class ReportService
    {
        private readonly SalesOrderRepository _salesRepository;
        private readonly RawMaterialRepository _inventoryRepository;

        public ReportService(SalesOrderRepository salesRepository, RawMaterialRepository inventoryRepository)
        {
            _salesRepository = salesRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<string> GenerateSalesReportAsync()
        {
            var sales = await _salesRepository.GetAllOrdersAsync();
            return $"Sales Report: {sales.Count()} orders processed.";
        }

        public async Task<string> GenerateInventoryReportAsync()
        {
            var materials = await _inventoryRepository.GetAllRawMaterialsAsync();
            return $"Inventory Report: {materials.Count()} materials in stock.";
        }
    }
}
