using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(string username);
    Task<IEnumerable<User>> GetUserByUserRoleAsync(UserRole role);
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(Guid userId) => await _context.Users.FindAsync(userId);

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    //public async Task DeleteUserAsync(string username)
    //{
    //    var user = await GetUserByUsernameAsync(username);
    //    if (user != null)
    //    {
    //        _context.Users.Remove(user);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    public async Task DeleteUserAsync(string username)
    {
        var sql = "DELETE FROM Users WHERE Username = @username";

        await _context.Database.ExecuteSqlRawAsync(sql, new SqlParameter("@username", username));
    }

    public async Task<IEnumerable<User>> GetUserByUserRoleAsync(UserRole role)
    {
        return await _context.Users.Where(u => u.Role == role).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
