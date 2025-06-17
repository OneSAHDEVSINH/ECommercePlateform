namespace ECommercePlatform.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string? Name { get; private set; }
        public string? SKU { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsAvailable { get; private set; } = true;

        // Navigation properties
        public virtual Product? Product { get; private set; }
        public virtual ICollection<OrderItem>? OrderItems { get; private set; }

        private ProductVariant() { }
    }
}