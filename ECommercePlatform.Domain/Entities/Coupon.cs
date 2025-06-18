namespace ECommercePlatform.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal MaximumValue { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        // Navigation properties
        public virtual Product? Product { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }

        private Coupon() { }
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
}