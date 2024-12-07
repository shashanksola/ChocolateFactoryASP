
using System.ComponentModel.DataAnnotations;

namespace ChocolateFactory.Models
{
    public class FinishedGood
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        public required Guid BatchId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public required int Quantity { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public required DateTime PackagingDate { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Warehouse location cannot exceed 200 characters.")]
        public required string WarehouseLocation { get; set; }
    }

}