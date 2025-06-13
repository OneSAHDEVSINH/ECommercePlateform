using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> GetProductWithDetailsAsync(Guid id);
    }
}