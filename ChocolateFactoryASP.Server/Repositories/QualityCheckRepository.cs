using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IQualityCheckRepository
    {
        Task<QualityCheck> GetCheckByIdAsync(int checkId);
        Task<IEnumerable<QualityCheck>> GetAllChecksAsync();
        Task AddCheckAsync(QualityCheck check);
        Task UpdateCheckAsync(QualityCheck check);
        Task DeleteCheckAsync(int checkId);
    }

    public class QualityCheckRepository : IQualityCheckRepository
    {
        private readonly ApplicationDbContext _context;

        public QualityCheckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QualityCheck> GetCheckByIdAsync(int checkId) =>
            await _context.QualityChecks.FindAsync(checkId);

        public async Task<IEnumerable<QualityCheck>> GetAllChecksAsync() =>
            await _context.QualityChecks.ToListAsync();

        public async Task AddCheckAsync(QualityCheck check)
        {
            await _context.QualityChecks.AddAsync(check);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCheckAsync(QualityCheck check)
        {
            _context.QualityChecks.Update(check);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCheckAsync(int checkId)
        {
            var check = await GetCheckByIdAsync(checkId);
            if (check != null)
            {
                _context.QualityChecks.Remove(check);
                await _context.SaveChangesAsync();
            }
        }
    }

}
