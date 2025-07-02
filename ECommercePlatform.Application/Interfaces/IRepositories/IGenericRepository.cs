using ECommercePlatform.Application.Models;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetActiveAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        // Unified paging method with support for search
        Task<PagedResponse<T>> GetPagedAsync(
            PagedRequest request,
            Expression<Func<T, bool>>? baseFilter = null,
            Func<IQueryable<T>, string?, IQueryable<T>>? searchFunction = null,
            CancellationToken cancellationToken = default);

    }
}