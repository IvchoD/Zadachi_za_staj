using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    // ==========================
    // Update Cart Quantity
    // ==========================

    public class UpdateCartQuantityDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}