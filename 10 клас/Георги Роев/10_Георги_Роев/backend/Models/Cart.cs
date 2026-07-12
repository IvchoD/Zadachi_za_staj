using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Shopping Cart
    // ==========================

    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new();
    }
}