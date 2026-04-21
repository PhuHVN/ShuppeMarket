namespace ShuppeMarket.Application.DTOs.ProductDtos
{
    public class ProductResponse
    {
        public string Id { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public IList<string> CategoryNames { get; set; } = new List<string>();




    }
}
