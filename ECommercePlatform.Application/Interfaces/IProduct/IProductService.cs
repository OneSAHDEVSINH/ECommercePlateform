using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IProduct
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<ProductDto> GetProductWithDetailsAsync(Guid id);
        Task<IReadOnlyList<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(Guid id);
    }
}