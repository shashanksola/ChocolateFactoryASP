using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;
using System.ComponentModel.DataAnnotations;
using ChocolateFactory.Requests;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly NotificationService _notificationService;

        public AuthController(AuthService authService, NotificationService notificationService)
        {
            _authService = authService;
            _notificationService = notificationService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var user = await _authService.AuthenticateUserAsync(loginRequest.Username, loginRequest.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        [Authorize(Roles ="FactoryManager")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest registerRequest)
        {

            var newUser = new User
            {
                Username = registerRequest.Username,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                PasswordHash = registerRequest.Password, 
                Email = registerRequest.Email,
                Role = registerRequest.Role
            };

            var success = await _authService.RegisterUserAsync(newUser);

            if (!success) return BadRequest(new { message = "User with username already exists" });

            string sub = "Dear new user, you have been successfully registered, your credentials are Username: " + newUser.Username + ", Password: " + registerRequest.Password;
            await _notificationService.SendEmailAsync(newUser.Email, "Welcome to Choco.co", sub);

            return Ok();
        }

        [HttpPost("registerNew")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserRegisterRequest registerRequest)
        {
            var newUser = new User
            {
                Username = registerRequest.Username,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                PasswordHash = registerRequest.Password,
                Email = registerRequest.Email,
                Role = registerRequest.Role,
                IsActive = false
            };

            var success = await _authService.RegisterUserAsync(newUser);

            if (!success) return BadRequest(new { message = "User with username already exists" });

            string sub = "Dear new user, you have been successfully registered, your credentials are Username: " + newUser.Username + ", Password: " + registerRequest.Password + " . Plese wait till the Factory Manager authorizer your request.";
            await _notificationService.SendEmailAsync(newUser.Email, "Welcome to Choco.co", sub);

            return Ok();
        }

    }


    public class UserRegisterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public required string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public required string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public required string LastName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

        [Required]
        public required UserRole Role { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }
    }
}