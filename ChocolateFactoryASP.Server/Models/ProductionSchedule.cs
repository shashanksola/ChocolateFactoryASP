
using System.ComponentModel.DataAnnotations;

namespace ChocolateFactory.Models
{
    public class ProductionSchedule
    {
        [Key]
        public Guid? ScheduleId { get; set; } = Guid.NewGuid();

        [Required]
        public required string RecipeName { get; set; } //recipe ID

        [Required]
        public required DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public required Shift Shift { get; set; }

        [Required]
        public required Guid SupervisorId { get; set; }

        [Required]
        public required ProductionStatus Status { get; set; }
    }
}