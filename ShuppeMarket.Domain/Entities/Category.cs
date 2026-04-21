using ShuppeMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShuppeMarket.Domain.Entities
{
    public class Category
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public StatusEnum Status { get; set; }
        // Navigation property 
        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
    }
}
