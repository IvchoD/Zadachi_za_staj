using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using backend.DTOs;

namespace backend.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        // ==========================
        // Get Cart
        // ==========================

        public async Task<Cart?> GetCartAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // ==========================
        // Create Cart
        // ==========================

        public async Task<Cart> CreateCartAsync(int userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        // ==========================
        // Add To Cart
        // ==========================

      public async Task<string> AddToCartAsync(AddToCartDto dto)
      {
     var product = await _context.Products.FindAsync(dto.ProductId);

     if (product == null)
         return "PRODUCT_NOT_FOUND";

     if (!product.IsAvailable)
         return "OUT_OF_STOCK";

     var cart = await GetCartAsync(dto.UserId);

     if (cart == null)
         cart = await CreateCartAsync(dto.UserId);
 
     var existingItem = cart.Items
         .FirstOrDefault(i => i.ProductId == dto.ProductId);

     if (existingItem != null)
     {
         if (existingItem.Quantity + dto.Quantity > product.Quantity)
             return "NOT_ENOUGH_STOCK";

         existingItem.Quantity += dto.Quantity;
     }
     else
     {
         if (dto.Quantity > product.Quantity)
             return "NOT_ENOUGH_STOCK";

         cart.Items.Add(new CartItem
         {
             ProductId = dto.ProductId,
             Quantity = dto.Quantity
         });
     }

     await _context.SaveChangesAsync();

      return "SUCCESS";
     }

     // ==========================
     // Get Cart Items
     // ==========================

      public async Task<Cart?> GetCartWithProductsAsync(int userId)
     {
        return await _context.Carts
        .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);
      }

      // ==========================
      // Update Quantity
      // ==========================

    public async Task<string> UpdateQuantityAsync(UpdateCartQuantityDto dto)
   {
      var cart = await GetCartAsync(dto.UserId);

      if (cart == null)
        return "CART_NOT_FOUND";

      var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

      if (item == null)
        return "ITEM_NOT_FOUND";

      var product = await _context.Products.FindAsync(dto.ProductId);
 
     if (product == null)
        return "PRODUCT_NOT_FOUND";

      if (dto.Quantity <= 0)
       {
        _context.CartItems.Remove(item);

        await _context.SaveChangesAsync();

        return "REMOVED";
       }

      if (dto.Quantity > product.Quantity)
        return "NOT_ENOUGH_STOCK";

       item.Quantity = dto.Quantity;

       await _context.SaveChangesAsync();

       return "SUCCESS";
    }

    // ==========================
    // Clear Cart
    // ==========================

   public async Task<bool> ClearCartAsync(int userId)
   {
      var cart = await GetCartAsync(userId);

      if (cart == null)
        return false;

      _context.CartItems.RemoveRange(cart.Items);

       await _context.SaveChangesAsync();

       return true;
    }
    // ==========================
    // Cart Summary
    // ==========================

   public async Task<object?> GetCartSummaryAsync(int userId)
   {
      var cart = await GetCartWithProductsAsync(userId);

      if (cart == null)
        return null;

      var items = cart.Items.Select(i => new
     {
        ProductId = i.ProductId,
        Name = i.Product.Name,
        Image = i.Product.ImagePath,
        Price = i.Product.Price,
        Quantity = i.Quantity,
        Total = i.Product.Price * i.Quantity
      }).ToList();

     decimal subtotal = items.Sum(i => i.Total);
 
     decimal shipping = subtotal >= 12m ? 0m : 4m;

     decimal grandTotal = subtotal + shipping;

      return new
     {
        Items = items,
        Subtotal = subtotal,
        Shipping = shipping,
        GrandTotal = grandTotal
      };
   }
    }
}