using ChocolateFactory.Models;
using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class MaintenanceService
    {
        private readonly MaintenanceRecordRepository _repository;

        public MaintenanceService(MaintenanceRecordRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync()
        {
            return await _repository.GetAllMaintenanceRecordsAsync();
        }

        public async Task AddMaintenanceRecordAsync(MaintenanceRecord record)
        {
            await _repository.AddMaintenanceRecordAsync(record);
        }

        public async Task UpdateMaintenenceRecordAsync(MaintenanceRecord record)
        {
            await _repository.UpdateMaintenanceRecordAsync(record);
        }

        public async Task DeleteMaintaneneceTask(Guid id)
        {
            await _repository.DeleteMaintenanceRecordAsync(id);
        }

        public async Task<MaintenanceRecord> GetMaintenanceRecordByIdAsync(Guid id)
        {
            return await _repository.GetMaintenanceRecordByIdAsync(id);
        }
    }
}
