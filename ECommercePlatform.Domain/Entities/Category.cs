namespace ECommercePlatform.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }

        // Navigation properties
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category>? Subcategories { get; set; }
        public virtual ICollection<Product>? Products { get; set; }

        private Category() { }
    }
}