using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IFinishedGoodsRepository
    {
        Task<FinishedGood> GetFinishedGoodByIdAsync(Guid productId);
        Task<IEnumerable<FinishedGood>> GetAllFinishedGoodsAsync();
        Task AddFinishedGoodAsync(FinishedGood finishedGood);
        Task UpdateFinishedGoodAsync(FinishedGood finishedGood);
        Task DeleteFinishedGoodAsync(Guid productId);
    }

    public class FinishedGoodsRepository : IFinishedGoodsRepository
    {
        private readonly ApplicationDbContext _context;

        public FinishedGoodsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FinishedGood> GetFinishedGoodByIdAsync(Guid productId) =>
            await _context.FinishedGoods.FindAsync(productId);

        public async Task<IEnumerable<FinishedGood>> GetAllFinishedGoodsAsync() =>
            await _context.FinishedGoods.ToListAsync();

        public async Task AddFinishedGoodAsync(FinishedGood finishedGood)
        {
            await _context.FinishedGoods.AddAsync(finishedGood);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFinishedGoodAsync(FinishedGood finishedGood)
        {
            _context.FinishedGoods.Update(finishedGood);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFinishedGoodAsync(Guid productId)
        {
            var finishedGood = await GetFinishedGoodByIdAsync(productId);
            if (finishedGood != null)
            {
                _context.FinishedGoods.Remove(finishedGood);
                await _context.SaveChangesAsync();
            }
        }
    }

}
