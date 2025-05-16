using System;
using System.Collections.Generic;

namespace ECommercePlatform.Server.Core.Domain.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public byte[]? Image { get; set; }
        public Guid? CategoryId { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public ICollection<ProductVariant>? Variants { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Coupon>? Coupons { get; set; }
    }
} 