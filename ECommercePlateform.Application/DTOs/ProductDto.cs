namespace ECommercePlateform.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
        public List<ProductVariantDto>? Variants { get; set; }
    }

    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateProductDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class UpdateProductDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public Guid? CategoryId { get; set; }
    }
}