using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // ==========================
    // User
    // ==========================
    public class User
    {
        [Key]
        public int Id { get; set; }

        // ==========================
        // Account Information
        // ==========================

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        // ==========================
        // Verification
        // ==========================

        public bool EmailVerified { get; set; } = false;

        // ==========================
        // Role
        // ==========================

        public bool IsAdmin { get; set; } = false;

        // ==========================
        // Account Information
        // ==========================

        public List<Cart> Carts { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}