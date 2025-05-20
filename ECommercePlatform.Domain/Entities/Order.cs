namespace ECommercePlatform.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string? OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public decimal SubTotal { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal ShippingAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public Guid? ShippingAddressId { get; private set; }
        public Guid? CouponId { get; private set; }

        // Navigation properties
        public User? User { get; private set; }
        public ShippingAddress? ShippingAddress { get; private set; }
        public Coupon? Coupon { get; private set; }
        public ICollection<OrderItem>? OrderItems { get; private set; }

        private Order() { }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }
}