using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IMaintenanceRecordRepository
    {
        Task<MaintenanceRecord> GetMaintenanceRecordByIdAsync(Guid recordId);
        Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync();
        Task AddMaintenanceRecordAsync(MaintenanceRecord record);
        Task UpdateMaintenanceRecordAsync(MaintenanceRecord record);
        Task DeleteMaintenanceRecordAsync(Guid recordId);
    }

    public class MaintenanceRecordRepository : IMaintenanceRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MaintenanceRecord> GetMaintenanceRecordByIdAsync(Guid recordId) =>
            await _context.MaintenanceRecords.FindAsync(recordId);

        public async Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync() =>
            await _context.MaintenanceRecords.ToListAsync();

        public async Task AddMaintenanceRecordAsync(MaintenanceRecord record)
        {
            await _context.MaintenanceRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMaintenanceRecordAsync(MaintenanceRecord record)
        {
            _context.MaintenanceRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMaintenanceRecordAsync(Guid recordId)
        {
            var record = await GetMaintenanceRecordByIdAsync(recordId);
            if (record != null)
            {
                _context.MaintenanceRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }

}
