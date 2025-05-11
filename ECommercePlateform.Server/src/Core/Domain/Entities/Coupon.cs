using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal MaximumValue { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

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