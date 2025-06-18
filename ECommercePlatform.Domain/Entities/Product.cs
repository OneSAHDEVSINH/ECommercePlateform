namespace ECommercePlatform.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public byte[]? Image { get; set; }
        public Guid? CategoryId { get; set; }

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductVariant>? Variants { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Coupon>? Coupons { get; set; }

        public Product() { }
    }
}