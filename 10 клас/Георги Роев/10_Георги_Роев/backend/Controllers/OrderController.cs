using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);

            return result switch
            {
                "SUCCESS" => Ok(new
                {
                    success = true,
                    message = "Order created successfully."
                }),

                "EMPTY_CART" => BadRequest(new
                {
                    success = false,
                    message = "Your cart is empty."
                }),

                _ => BadRequest(new
                {
                    success = false,
                    message = "Unknown error."
                })
            };
        }
    }
}