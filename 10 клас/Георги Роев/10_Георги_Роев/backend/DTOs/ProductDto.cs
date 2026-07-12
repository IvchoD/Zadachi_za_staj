namespace backend.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Ingredients { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public List<string> Badges { get; set; } = new();
    }
}