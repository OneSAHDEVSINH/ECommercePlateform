using ECommercePlatform.Server.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePlatform.Server.Core.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> GetProductWithDetailsAsync(Guid id);
    }
} 