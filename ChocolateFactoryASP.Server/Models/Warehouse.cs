
using System.ComponentModel.DataAnnotations;


namespace ChocolateFactory.Models
{
    public class Warehouse
    {

        [Required]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public required string Location { get; set; }
        
        [Key]
        public required string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public required int Capacity { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Current stock level cannot be negative.")]
        public int CurrentStockLevel { get; set; }
    }
}