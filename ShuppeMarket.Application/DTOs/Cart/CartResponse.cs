namespace ShuppeMarket.Application.DTOs.Cart
{
    public class CartResponse
    {
        public string CartId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public List<CartDetailResponse> CartDetails { get; set; } = new();

    }
}
