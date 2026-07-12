using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // Contact Message
    // ==========================
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        // ==========================
        // Sender Information
        // ==========================

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // ==========================
        // Message
        // ==========================

        [Required]
        [MaxLength(3000)]
        public string Message { get; set; } = string.Empty;

        // ==========================
        // Date
        // ==========================

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}