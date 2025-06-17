namespace ECommercePlatform.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }
        public string? ProductName { get; private set; }
        public string? VariantName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalPrice { get; private set; }

        // Navigation properties
        public virtual Order? Order { get; private set; }
        public virtual Product? Product { get; private set; }
        public virtual ProductVariant? ProductVariant { get; private set; }

        private OrderItem() { }
    }
}