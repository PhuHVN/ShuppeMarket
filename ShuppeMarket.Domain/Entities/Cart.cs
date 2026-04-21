namespace ShuppeMarket.Domain.Entities
{
    public class Cart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsActive { get; set; } = true;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        // Navigation properties
        public string AccountId { get; set; } = null!;
        public Account Account { get; set; } = null!;
        public List<CartDetail> CartDetails { get; set; } = new();
    }
}
