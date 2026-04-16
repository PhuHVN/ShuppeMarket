using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.DTOs.Cart
{
    public class CartRequest
    {
        public IList<ListProduct> Products { get; set; } = new List<ListProduct>();
    }
    public class ListProduct
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        
    }
}
