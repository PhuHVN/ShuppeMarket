namespace ShuppeMarket.Application.DTOs.SellerDtos
{
    public class SellerUpdateRequest
    {
        public string ShopName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }
}
