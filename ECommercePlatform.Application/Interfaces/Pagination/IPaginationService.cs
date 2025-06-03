using ECommercePlatform.Application.Models;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.Pagination
{
    public interface IPaginationService
    {
        Task<PagedResponse<TDto>> CreatePagedResponseAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            PagedRequest request,
            Func<TEntity, TDto> mapFunction,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            string sortColumn,
            string? sortDirection = "asc");

        // Helper to combine search predicates with OR
        Expression<Func<T, bool>> CombineSearchPredicates<T>(
            params Expression<Func<T, bool>>[] predicates);
    }
}