using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.Entities
{
    public class Review
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        //navigation properties
        public string ProductId { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;
        public string AccountId { get; set; } = string.Empty;
        public Account Account { get; set; } = null!;
    }
}
