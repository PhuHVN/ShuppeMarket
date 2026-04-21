namespace ShuppeMarket.Domain.Entities
{
    public class CartDetail
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal PriceAtTime { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public string CartId { get; set; } = null!;
        public Cart Cart { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
