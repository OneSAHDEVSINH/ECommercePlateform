namespace ECommercePlatform.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Code { get; private set; }
        public string? Description { get; private set; }
        public decimal DiscountValue { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal MinimumValue { get; private set; }
        public decimal MaximumValue { get; private set; }
        public DateTime ValidFrom { get; private set; }
        public DateTime ValidTo { get; private set; }
        public Guid? ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }

        // Navigation properties
        public virtual Product? Product { get; private set; }
        public virtual ProductVariant? ProductVariant { get; private set; }
        public virtual ICollection<Order>? Orders { get; private set; }

        private Coupon() { }
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
}