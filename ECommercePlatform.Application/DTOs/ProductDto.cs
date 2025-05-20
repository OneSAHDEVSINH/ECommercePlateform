namespace ECommercePlatform.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? SKU { get; init; }
        public int StockQuantity { get; init; }
        public bool IsAvailable { get; init; }
        public Guid? CategoryId { get; init; }
        public string? CategoryName { get; init; }
        public bool IsActive { get; init; }
        public List<ProductVariantDto>? Variants { get; init; }
    }

    public class ProductVariantDto
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? SKU { get; init; }
        public decimal Price { get; init; }
        public int StockQuantity { get; init; }
        public bool IsAvailable { get; init; }
    }

    public class CreateProductDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? SKU { get; init; }
        public int StockQuantity { get; init; }
        public Guid? CategoryId { get; init; }
    }

    public class UpdateProductDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? SKU { get; init; }
        public int StockQuantity { get; init; }
        public bool IsAvailable { get; init; }
        public Guid? CategoryId { get; init; }
    }
}