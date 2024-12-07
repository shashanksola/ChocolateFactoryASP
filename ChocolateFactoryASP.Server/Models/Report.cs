
using System.ComponentModel.DataAnnotations;


namespace ChocolateFactory.Models
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Report type cannot exceed 100 characters.")]
        public required string Type { get; set; } // E.g., Production, Sales, Quality Control, Inventory

        [Required]
        public required DateTime GeneratedDate { get; set; }

        [Required]
        public required Guid CreatedById { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Report data cannot exceed 1000 characters.")]
        public required string Data { get; set; } // This could store serialized JSON or summary information
    }
}