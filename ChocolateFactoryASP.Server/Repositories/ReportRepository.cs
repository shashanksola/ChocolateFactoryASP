using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IReportRepository
    {
        Task<Report> GetReportByIdAsync(int reportId);
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task AddReportAsync(Report report);
        Task UpdateReportAsync(Report report);
        Task DeleteReportAsync(int reportId);
    }

    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Report> GetReportByIdAsync(int reportId) =>
            await _context.Reports.FindAsync(reportId);

        public async Task<IEnumerable<Report>> GetAllReportsAsync() =>
            await _context.Reports.ToListAsync();

        public async Task AddReportAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReportAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReportAsync(int reportId)
        {
            var report = await GetReportByIdAsync(reportId);
            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
    }

}
