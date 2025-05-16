using System;
using System.Collections.Generic;

namespace ECommercePlatform.Server.Core.Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }

        // Navigation properties
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? Subcategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
} 