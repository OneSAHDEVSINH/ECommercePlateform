using ECommercePlateform.Application.Interfaces.IGeneral;
using ECommercePlateform.Domain.Entities;

namespace ECommercePlateform.Application.Interfaces.IProduct
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> GetProductWithDetailsAsync(Guid id);
    }
}