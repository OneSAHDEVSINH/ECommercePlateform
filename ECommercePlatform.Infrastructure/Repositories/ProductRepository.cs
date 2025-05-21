using ECommercePlatform.Application.Interfaces.IProduct;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
    {
        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync() ?? new List<Product>();
        }

        public async Task<Product> GetProductWithDetailsAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants!.Where(v => v.IsActive && !v.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && !p.IsDeleted)
                ?? throw new InvalidOperationException($"Product with ID {id} not found.");

            return product!;
        }
    }
}