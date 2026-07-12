using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    // ==========================
    // Database Context
    // ==========================
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ==========================
        // Tables
        // ==========================

        public DbSet<Product> Products => Set<Product>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Badge> Badges => Set<Badge>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        
        public DbSet<Cart> Carts => Set<Cart>();

        public DbSet<CartItem> CartItems => Set<CartItem>();

        // ==========================
        // Relationships
        // ==========================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One badge belongs to one product
            modelBuilder.Entity<Badge>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Badges)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}