using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Order Item
    // ==========================
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        // ==========================
        // Order
        // ==========================

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        // ==========================
        // Product
        // ==========================

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        // ==========================
        // Quantity
        // ==========================

        public int Quantity { get; set; }

        // ==========================
        // Price at Purchase
        // ==========================

        public decimal UnitPrice { get; set; }
    }
}