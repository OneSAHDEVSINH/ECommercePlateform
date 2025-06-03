using ECommercePlatform.Application.Models;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.Pagination
{
    public interface IPaginationService
    {
        //Task<PagedResponse<TDto>> CreatePagedResponseAsync<TEntity, TDto>(
        //    IQueryable<TEntity> query,
        //    PagedRequest request,
        //    Func<TEntity, TDto> mapFunction,
        //    CancellationToken cancellationToken = default)
        //    where TEntity : class;

        //IQueryable<T> ApplyPaging<T>(
        //    IQueryable<T> query,
        //    int pageNumber,
        //    int pageSize);

        //IQueryable<T> ApplySearch<T>(
        //    IQueryable<T> query,
        //    string? searchText,
        //    params Expression<Func<T, bool>>[] searchPredicates);

        //IQueryable<T> ApplySort<T>(
        //    IQueryable<T> query,
        //    string? sortColumn,
        //    string? sortDirection,
        //    Dictionary<string, Expression<Func<T, object>>>? sortExpressions = null);

        ////alone method for creating paged result with filter expression and mapping function
        //Task<PagedResponse<TResult>> CreatePagedResultAsync<TEntity, TResult>(
        //    IQueryable<TEntity> query,
        //    PagedRequest request,
        //    Expression<Func<TEntity, bool>>? filterExpression = null,
        //    Func<TEntity, TResult> mappingFunction = null!)
        //    where TEntity : class
        //    where TResult : class;


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