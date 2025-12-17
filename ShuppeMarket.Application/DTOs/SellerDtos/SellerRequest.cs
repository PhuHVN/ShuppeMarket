using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.DTOs.SellerDtos
{
    public class SellerRequest
    {        
        public string AccountId { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LogoUrl { get; set; } = string.Empty;
    }
}
