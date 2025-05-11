using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Guid? ShippingAddressId { get; set; }
        public Guid? CouponId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public Coupon? Coupon { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
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