using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Badge
    // ==========================
    public class Badge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Product that owns this badge
        public int? ProductId { get; set; }

        public Product? Product { get; set; }
    }
}