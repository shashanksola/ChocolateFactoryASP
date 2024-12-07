
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ChocolateFactory.Models
{
    public class RawMaterial
    {

        [Key]
        public Guid RawMaterialBatchId { get; set; } = Guid.NewGuid();

        [Required]
        public required string WarehouseName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive number.")]
        public required int StockQuantity { get; set; }

        [Required]
        public required Unit Unit { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public required string SupplierName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public required decimal CostPerUnit { get; set; }
    }
}