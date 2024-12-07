using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface ISalesOrderRepository
    {
        Task<SalesOrder> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<SalesOrder>> GetAllOrdersAsync();
        Task AddOrderAsync(SalesOrder order);
        Task UpdateOrderAsync(SalesOrder order);
        Task DeleteOrderAsync(int orderId);
    }

    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public SalesOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalesOrder> GetOrderByIdAsync(int orderId) =>
            await _context.SalesOrders.FindAsync(orderId);

        public async Task<IEnumerable<SalesOrder>> GetAllOrdersAsync() =>
            await _context.SalesOrders.ToListAsync();

        public async Task AddOrderAsync(SalesOrder order)
        {
            await _context.SalesOrders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(SalesOrder order)
        {
            _context.SalesOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order != null)
            {
                _context.SalesOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }

}
