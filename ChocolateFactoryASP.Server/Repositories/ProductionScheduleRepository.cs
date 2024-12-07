using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IProductionScheduleRepository
    {
        Task<ProductionSchedule> GetScheduleByIdAsync(Guid scheduleId);
        Task<IEnumerable<ProductionSchedule>> GetAllSchedulesAsync();
        Task AddScheduleAsync(ProductionSchedule schedule);
        Task UpdateScheduleAsync(ProductionSchedule schedule);
        Task DeleteScheduleAsync(Guid scheduleId);
    }

    public class ProductionScheduleRepository : IProductionScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductionScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductionSchedule> GetScheduleByIdAsync(Guid scheduleId) =>
            await _context.ProductionSchedules.FindAsync(scheduleId);

        public async Task<IEnumerable<ProductionSchedule>> GetAllSchedulesAsync() =>
            await _context.ProductionSchedules.ToListAsync();

        public async Task AddScheduleAsync(ProductionSchedule schedule)
        {
            await _context.ProductionSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScheduleAsync(ProductionSchedule schedule)
        {
            _context.ProductionSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetScheduleByIdAsync(scheduleId);
            if (schedule != null)
            {
                _context.ProductionSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }

}
