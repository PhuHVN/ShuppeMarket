using ShuppeMarket.Domain.Enums;

namespace ShuppeMarket.Application.DTOs.SellerDtos
{
    public class SellerResponse
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LogoUrl { get; set; } = string.Empty;
        public RoleEnum Role { get; set; }
        public StatusEnum Status { get; set; }

    }
}
