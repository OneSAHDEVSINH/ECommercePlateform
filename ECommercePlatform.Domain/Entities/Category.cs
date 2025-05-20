namespace ECommercePlatform.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        // Navigation properties
        public Category? ParentCategory { get; private set; }
        public ICollection<Category>? Subcategories { get; private set; }
        public ICollection<Product>? Products { get; private set; }

        private Category() { }
    }
}