using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Shopping Cart Item
    // ==========================

    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}