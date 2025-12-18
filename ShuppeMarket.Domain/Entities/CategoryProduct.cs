using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.Entities
{
    public class CategoryProduct
    {
        public string CategoryId { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
