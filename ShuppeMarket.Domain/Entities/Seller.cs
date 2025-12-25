using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.Entities
{
    public class Seller
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AccountId { get; set; } = string.Empty;
        [Required]
        public string ShopName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? LogoUrl { get; set; } = string.Empty;

      
        // Navigation Properties
        public Account Account { get; private set; }
        // Products 
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
