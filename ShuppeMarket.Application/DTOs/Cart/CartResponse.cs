using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
