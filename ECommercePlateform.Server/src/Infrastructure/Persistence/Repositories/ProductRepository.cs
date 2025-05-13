using ECommercePlateform.Server.Core.Application.Interfaces;
using ECommercePlateform.Server.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted)
                .Include(p => p.Category)
                .ToListAsync() ?? new List<Product>();
        }

        public async Task<Product> GetProductWithDetailsAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants!.Where(v => v.IsActive && !v.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && !p.IsDeleted)
                ?? throw new InvalidOperationException($"Product with ID {id} not found.");

            return product;
        }
    }
} 