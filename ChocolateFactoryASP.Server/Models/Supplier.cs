
using System.ComponentModel.DataAnnotations;


namespace ChocolateFactory.Models
{
    public class Supplier
    {
        [Key]
        public Guid SupplierId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters.")]
        public required string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public required string Address { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public required string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}