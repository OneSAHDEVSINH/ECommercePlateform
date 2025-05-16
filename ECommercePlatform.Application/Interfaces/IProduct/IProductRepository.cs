using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces.IProduct
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> GetProductWithDetailsAsync(Guid id);
    }
}