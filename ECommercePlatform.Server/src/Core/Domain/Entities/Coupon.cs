using System;
using System.Collections.Generic;

namespace ECommercePlatform.Server.Core.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
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
        public Product? Product { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
} 