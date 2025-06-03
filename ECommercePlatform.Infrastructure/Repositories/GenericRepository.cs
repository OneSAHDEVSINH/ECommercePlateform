using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context = context;

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                .OrderByDescending(e => EF.Property<object>(e, "CreatedOn"))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id)
                ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
            _context.Entry(entity).State = EntityState.Detached; // Detach the entity to avoid tracking issues
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity != null;
        }

        // Implement the new method
        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? _context.Set<T>().AsQueryable()
                : _context.Set<T>().Where(predicate);
        }

        // Streamlined paging method with support for search
        public async Task<PagedResponse<T>> GetPagedAsync(
            PagedRequest request,
            Expression<Func<T, bool>>? baseFilter = null,
            Func<IQueryable<T>, string?, IQueryable<T>>? searchFunction = null,
            CancellationToken cancellationToken = default)
        {
            // Start with base query
            var query = _context.Set<T>().AsQueryable();

            // Apply base filter if provided
            if (baseFilter != null)
                query = query.Where(baseFilter);

            // Apply search if provided
            if (searchFunction != null && !string.IsNullOrWhiteSpace(request.SearchText))
                query = searchFunction(query, request.SearchText);

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting or use default
            if (!string.IsNullOrWhiteSpace(request.SortColumn))
                query = query.ApplyDynamicOrderBy(request.SortColumn, request.SortDirection ?? "asc");

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Return paged response
            return new PagedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}