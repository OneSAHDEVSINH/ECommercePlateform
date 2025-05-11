using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? Subcategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
} 