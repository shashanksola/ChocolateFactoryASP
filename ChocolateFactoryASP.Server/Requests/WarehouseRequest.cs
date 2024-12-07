using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ChocolateFactory.Requests
{
    public class WarehouseRequest
    {
        public required string Location { get; set; }

        [Required]
        [Key]
        public required string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public required int Capacity { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Current stock level cannot be negative.")]
        public int CurrentStockLevel { get; set; } = 0;
    }
}
