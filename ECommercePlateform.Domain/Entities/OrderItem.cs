namespace ECommercePlateform.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string? ProductName { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public ProductVariant? ProductVariant { get; set; }
    }
}