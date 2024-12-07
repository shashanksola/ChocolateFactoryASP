using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            return await _context.Notifications.FindAsync(notificationId);
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync() =>
            await _context.Notifications.ToListAsync();

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await GetNotificationByIdAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }

}
