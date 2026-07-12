using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Order
    // ==========================
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // ==========================
        // Customer
        // ==========================

        public int UserId { get; set; }

        public User? User { get; set; }

        // ==========================
        // Order Information
        // ==========================

        [Required]
        public decimal ProductsTotal { get; set; }

        [Required]
        public decimal ShippingPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        // ==========================
        // Delivery
        // ==========================

        [Required]
        public string DeliveryCompany { get; set; } = string.Empty;

        // ==========================
        // Payment
        // ==========================

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        // ==========================
        // Order Status
        // ==========================

        public string Status { get; set; } = "Order Sent";

        // ==========================
        // Date
        // ==========================

        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}