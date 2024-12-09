
using System.ComponentModel.DataAnnotations;


namespace ChocolateFactory.Models
{
    public class SalesOrder
    {
        [Key]
        public Guid? OrderId { get; set; } = Guid.NewGuid();

        [Required]
        public required Guid CustomerId { get; set; }

        [Required]
        public required Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public required int Quantity { get; set; }

        [Required]
        public required DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public required string Status { get; set; }
    }
}