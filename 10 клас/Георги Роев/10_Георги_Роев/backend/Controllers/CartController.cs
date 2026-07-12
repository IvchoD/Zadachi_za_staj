using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // ==========================
    // Shopping Cart Controller
    // ==========================

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // ==========================
        // Add To Cart
        // ==========================

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(dto);

            return result switch
            {
                "SUCCESS" => Ok(new
                {
                    success = true,
                    message = "Product added to cart."
                }),

                "PRODUCT_NOT_FOUND" => NotFound(new
                {
                    success = false,
                    message = "Product not found."
                }),

                "OUT_OF_STOCK" => BadRequest(new
                {
                    success = false,
                    message = "Product is out of stock."
                }),

                "NOT_ENOUGH_STOCK" => BadRequest(new
                {
                    success = false,
                    message = "Not enough stock available."
                }),

                _ => BadRequest(new
                {
                    success = false,
                    message = "Unknown error."
                })
            };
        }

        // ==========================
        // Get Cart
        // ==========================

     [HttpGet("{userId}")]
     public async Task<IActionResult> GetCart(int userId)
     {
       var cart = await _cartService.GetCartWithProductsAsync(userId);

      if (cart == null)
      {
          return Ok(new
          {
              success = true,
              items = new List<object>()
          });
      }

     var items = cart.Items.Select(i => new
       {
          i.ProductId,
          ProductName = i.Product.Name,
          i.Product.ImagePath,
          Price = i.Product.Price,
          i.Quantity,
          Total = i.Product.Price * i.Quantity
      });

      return Ok(new
      {
          success = true,
          items
      });
     }


     // ==========================
     // Update Quantity
     // ==========================

     [HttpPut("quantity")]
    public async Task<IActionResult> UpdateQuantity(UpdateCartQuantityDto dto)
    {
      var result = await _cartService.UpdateQuantityAsync(dto);

      return result switch
      {
          "SUCCESS" => Ok(new
          {
            success = true,
            message = "Quantity updated."
          }),

          "REMOVED" => Ok(new
          {
            success = true,
             message = "Item removed from cart."
          }),

          "NOT_ENOUGH_STOCK" => BadRequest(new
         {
            success = false,
            message = "Not enough stock."
          }),

          "ITEM_NOT_FOUND" => NotFound(new
          {
            success = false,
            message = "Item not found."
         }),

         "CART_NOT_FOUND" => NotFound(new
         {
             success = false,
            message = "Cart not found."
         }),
 
         _ => BadRequest(new
         {
            success = false,
            message = "Unknown error."
          })
       };
    }

    // ==========================
    // Clear Cart
    // ==========================

    [HttpDelete("clear/{userId}")]
   public async Task<IActionResult> ClearCart(int userId)
   {
     var success = await _cartService.ClearCartAsync(userId);

      if (!success)
     {
        return NotFound(new
        {
            success = false,
            message = "Cart not found."
        });
     }

     return Ok(new
      {
          success = true,
         message = "Cart cleared successfully."
       });
    }

    // ==========================
    // Cart Summary
    // ==========================

   [HttpGet("summary/{userId}")]
   public async Task<IActionResult> GetSummary(int userId)
   {
     var summary = await _cartService.GetCartSummaryAsync(userId);

      if (summary == null)
     {
        return NotFound(new
        {
            success = false,
            message = "Cart not found."
        });
      }

      return Ok(summary);
    }
    }
}