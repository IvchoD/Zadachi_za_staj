using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateOrderAsync(CreateOrderDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

            if (cart == null || !cart.Items.Any())
                return "EMPTY_CART";

            decimal productsTotal = cart.Items.Sum(i => i.Product.Price * i.Quantity);

            decimal shipping = productsTotal >= 12m ? 0m : 4m;

            var order = new Order
            {
                UserId = dto.UserId,
                ProductsTotal = productsTotal,
                ShippingPrice = shipping,
                TotalPrice = productsTotal + shipping,
                DeliveryCompany = dto.DeliveryCompany,
                PaymentMethod = dto.PaymentMethod,
                Status = "Order Sent",
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            foreach (var item in cart.Items)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });

                item.Product.Quantity -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            return "SUCCESS";
        }
    }
}