
using System.ComponentModel.DataAnnotations;

namespace ChocolateFactory.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }

        [Required]
        public required NotificationType Type { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public required string Message { get; set; }

        [Required]
        public required DateTime Timestamp { get; set; }

        [Required]
        public required Guid UserId { get; set; }
    }
}
