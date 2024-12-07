using BCrypt.Net;
using ChocolateFactory.Models;
using ChocolateFactory.Repositories;
using ChocolateFactory.Helpers;

namespace ChocolateFactory.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IEnumerable<User>> GetUsersByUserRoleAsync(UserRole role) {
            return await _userRepository.GetUserByUserRoleAsync(role);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task DeleteUserAsync(string username)
        {
            await _userRepository.DeleteUserAsync(username);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }
    }
}
