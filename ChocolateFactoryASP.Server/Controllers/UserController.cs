using ChocolateFactory.Models;
using ChocolateFactory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChocolateFactory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet("{role}")]
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _service.GetUsersByUserRoleAsync(role);
            return users;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _service.GetUsersAsync();
            return users;
        }

        [HttpDelete("{username}")]
        [Authorize(Roles ="FactoryManager")]
        public async Task<IActionResult> DeleteUserAsync(string username)
        {
            await _service.DeleteUserAsync(username);
            return Ok();
        }

        [HttpGet("id/{id}")]
        public async Task<User> GetUserById(Guid id) {
            return await _service.GetUserByIdAsync(id);
        }
    }
}
