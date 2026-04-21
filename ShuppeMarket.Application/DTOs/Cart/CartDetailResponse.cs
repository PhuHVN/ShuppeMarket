namespace ShuppeMarket.Application.DTOs.Cart
{
    public class CartDetailResponse
    {
        public string CartDetailId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string ShopId { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
