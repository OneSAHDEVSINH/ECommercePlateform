namespace ECommercePlatform.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        public virtual Product? Product { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }

        private ProductVariant() { }
    }
}