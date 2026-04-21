using Microsoft.AspNetCore.Http;

namespace ShuppeMarket.Application.DTOs.ProductDtos
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public IList<string> CategoryIds { get; set; } = new List<string>();
        public IFormFile? Image { get; set; }
    }
}
