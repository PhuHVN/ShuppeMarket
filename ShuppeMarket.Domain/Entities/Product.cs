using ShuppeMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShuppeMarket.Domain.Entities
{
    public class Product
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public StatusEnum Status { get; set; }
        // Navigation property
        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
        // Seller relationship
        public string SellerId { get; set; } = string.Empty;
        public Seller Seller { get; private set; } = null!;


    }
}
