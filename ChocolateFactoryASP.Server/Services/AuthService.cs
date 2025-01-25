using BCrypt.Net;
using ChocolateFactory.Models;
using ChocolateFactory.Repositories;
using ChocolateFactory.Helpers;

namespace ChocolateFactory.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(UserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return null;

            // Verify the password using BCrypt
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isPasswordValid ? user : null;
        }

        public async Task<bool> RegisterUserAsync(User newUser)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(newUser.Username);
            if (existingUser != null)
            {
                return false;
            }
            existingUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                return false;
            }

            // Hash the password using BCrypt before storing
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            await _userRepository.AddUserAsync(newUser);
            return true;
        }

        public string GenerateJwtToken(User user)
        {
            // Delegate token generation to JwtHelper
            return _jwtHelper.GenerateJwtToken(user);
        }
    }
}
