using ECommercePlatform.Application.Models;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IGeneral
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Guid id);

        // Add pagination method
        IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null);

        // Unified paging method with support for search
        Task<PagedResponse<T>> GetPagedAsync(
            PagedRequest request,
            Expression<Func<T, bool>>? baseFilter = null,
            Func<IQueryable<T>, string?, IQueryable<T>>? searchFunction = null,
            CancellationToken cancellationToken = default);

    }
}