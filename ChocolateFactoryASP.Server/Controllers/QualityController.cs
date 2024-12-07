using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "QualityController,FactoryManager")]
    public class QualityController : ControllerBase
    {
        private readonly QualityControlService _service;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public QualityController(QualityControlService service, UserService userService, NotificationService notificationService)
        {
            _service = service;
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = "PackagingStaff")]
        public async Task<IActionResult> GetAllChecks()
        {
            var checks = await _service.GetAllChecksAsync();
            return Ok(checks);
        }

        [HttpPost]
        public async Task<IActionResult> AddQualityCheck([FromBody] QualityCheck check)
        {
            IEnumerable<User> users = await _userService.GetUsersByUserRoleAsync(UserRole.PackagingStaff);

            // Prepare email content
            string subject = "Quality Check Done!";
            string body = $"A QC with '{check.Status}' status has been added to the Chocolate Factory system.";

            // Send email notifications
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _notificationService.SendEmailAsync(user.Email, subject, body);
                }
            }

            await _service.AddQualityCheckAsync(check);
            return Ok(check);
        }
    }
}