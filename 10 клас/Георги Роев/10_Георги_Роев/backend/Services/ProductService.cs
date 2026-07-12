using backend.Data;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Badges)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    Ingredients = p.Ingredients,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Badges = p.Badges
                        .Select(b => b.Name)
                        .ToList()
                })
                .ToListAsync();
        }
          // ==========================
          // Get Product By Id
        // ==========================

      public async Task<ProductDto?> GetProductByIdAsync(int id)
{
    return await _context.Products
        .Include(p => p.Badges)
        .Where(p => p.Id == id)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description,
            Ingredients = p.Ingredients,
            ImagePath = p.ImagePath,
            IsAvailable = p.IsAvailable,
            Badges = p.Badges.Select(b => b.Name).ToList()
        })
        .FirstOrDefaultAsync();
}

// ==========================
// Search Products
// ==========================

public async Task<List<ProductDto>> SearchProductsAsync(string query)
{
    query = query.ToLower();

    return await _context.Products
        .Include(p => p.Badges)
        .Where(p => p.Name.ToLower().Contains(query))
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description,
            Ingredients = p.Ingredients,
            ImagePath = p.ImagePath,
            IsAvailable = p.IsAvailable,
            Badges = p.Badges.Select(b => b.Name).ToList()
        })
        .ToListAsync();
}

// ==========================
// Available Products
// ==========================

public async Task<List<ProductDto>> GetAvailableProductsAsync()
{
    return await _context.Products
        .Include(p => p.Badges)
        .Where(p => p.IsAvailable && p.Quantity > 0)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description,
            Ingredients = p.Ingredients,
            ImagePath = p.ImagePath,
            IsAvailable = p.IsAvailable,
            Badges = p.Badges.Select(b => b.Name).ToList()
        })
        .ToListAsync();
}
    }
}