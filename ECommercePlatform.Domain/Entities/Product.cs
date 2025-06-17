namespace ECommercePlatform.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public string? SKU { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public byte[]? Image { get; private set; }
        public Guid? CategoryId { get; private set; }

        // Navigation properties
        public virtual Category? Category { get; private set; }
        public virtual ICollection<ProductVariant>? Variants { get; private set; }
        public virtual ICollection<OrderItem>? OrderItems { get; private set; }
        public virtual ICollection<Review>? Reviews { get; private set; }
        public virtual ICollection<Coupon>? Coupons { get; private set; }

        public Product() { }
    }
}