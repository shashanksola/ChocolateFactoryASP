
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ChocolateFactory.Models
{
    public class QualityCheck
    {
        [Key]
        public Guid CheckId { get; set; }

        [Required]
        public required Guid BatchId { get; set; }

        [Required]
        public required Guid QualityControllerId { get; set; }

        [Required]
        public required DateTime InspectionDate { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Test results cannot exceed 500 characters.")]
        public required string TestResults { get; set; }

        [Required]
        public required QualityStatus Status { get; set; }
    }
}