using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // ==========================
    // Products Controller
    // ==========================

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // ==========================
        // GET: api/products
        // Returns all products
        // ==========================

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

  // ==========================
  // GET Product By Id
  // ==========================

  [HttpGet("{id}")]
  public async Task<ActionResult<ProductDto>> GetProduct(int id)
  {
    var product = await _productService.GetProductByIdAsync(id);

    if (product == null)
        return NotFound();

    return Ok(product);
   }

// ==========================
// Search Products
// ==========================

[HttpGet("search")]
public async Task<ActionResult<List<ProductDto>>> SearchProducts([FromQuery] string query)
{
    var products = await _productService.SearchProductsAsync(query);

    return Ok(products);
}

// ==========================
// Available Products
// ==========================

[HttpGet("available")]
public async Task<ActionResult<List<ProductDto>>> AvailableProducts()
{
    var products = await _productService.GetAvailableProductsAsync();

    return Ok(products);
}
    }
}