using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            await _notificationService.SendEmailAsync(request.ToEmail, request.Subject, request.Message);
            return Ok(new { message = "Notification sent successfully." });
        }
    }

    public class NotificationRequest
    {
        public required string ToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
}