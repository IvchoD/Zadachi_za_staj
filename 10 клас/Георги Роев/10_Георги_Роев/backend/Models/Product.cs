using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Product
    // ==========================
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Ingredients { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string ImagePath { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;


        public List<CartItem> CartItems { get; set; } = new();

        // ==========================
        // Product Badges
        // ==========================

        public List<Badge> Badges { get; set; } = new();
    }
}