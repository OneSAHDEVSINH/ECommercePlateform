using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Common.Helpers;
using ECommercePlatform.Application.Interfaces.Pagination;
using ECommercePlatform.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ECommercePlatform.Application.Services
{
    public class PaginationService : IPaginationService
    {
        //public async Task<PagedResponse<TDto>> CreatePagedResponseAsync<TEntity, TDto>(
        //    IQueryable<TEntity> query,
        //    PagedRequest request,
        //    Func<TEntity, TDto> mapFunction,
        //    CancellationToken cancellationToken = default) where TEntity : class
        //{
        //    // Get total count for pagination
        //    var totalCount = await query.CountAsync(cancellationToken);

        //    // Apply paging to query
        //    var pagedItems = await ApplyPaging(query, request.PageNumber, request.PageSize)
        //        .AsNoTracking()
        //        .ToListAsync(cancellationToken);

        //    // Map to DTOs
        //    var mappedItems = pagedItems.Select(mapFunction).ToList();

        //    // Create paged response
        //    return new PagedResponse<TDto>
        //    {
        //        Items = mappedItems,
        //        TotalCount = totalCount,
        //        PageNumber = request.PageNumber,
        //        PageSize = request.PageSize
        //    };
        //}

        //public IQueryable<T> ApplyPaging<T>(IQueryable<T> query, int pageNumber, int pageSize)
        //{
        //    return query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize);
        //}

        //public IQueryable<T> ApplySearch<T>(IQueryable<T> query, string? searchText, params Expression<Func<T, bool>>[] searchPredicates)
        //{
        //    if (string.IsNullOrWhiteSpace(searchText) || searchPredicates.Length == 0)
        //        return query;

        //    // Combine all search predicates with OR
        //    Expression<Func<T, bool>>? combinedExpression = null;

        //    foreach (var predicate in searchPredicates)
        //    {
        //        if (combinedExpression == null)
        //            combinedExpression = predicate;
        //        else
        //            combinedExpression = PredicateBuilder.Or(combinedExpression, predicate);
        //    }

        //    if (combinedExpression != null)
        //        return query.Where(combinedExpression);

        //    return query;
        //}

        //public IQueryable<T> ApplySort<T>(
        //    IQueryable<T> query,
        //    string? sortColumn,
        //    string? sortDirection,
        //    Dictionary<string, Expression<Func<T, object>>>? sortExpressions = null)
        //{
        //    if (string.IsNullOrEmpty(sortColumn))
        //        return query;

        //    // Check if there's a predefined sort expression
        //    if (sortExpressions != null && sortExpressions.TryGetValue(sortColumn.ToLower(), out var sortExpression))
        //    {
        //        return sortDirection?.ToLower() == "desc"
        //            ? query.OrderByDescending(sortExpression)
        //            : query.OrderBy(sortExpression);
        //    }

        //    // Fallback to reflection-based sorting
        //    var property = typeof(T).GetProperty(
        //        sortColumn.First().ToString().ToUpper() + sortColumn[1..],
        //        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //    if (property != null)
        //    {
        //        var parameter = Expression.Parameter(typeof(T), "x");
        //        var propertyAccess = Expression.Property(parameter, property);
        //        var conversion = Expression.Convert(propertyAccess, typeof(object));
        //        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

        //        return sortDirection?.ToLower() == "desc"
        //            ? query.OrderByDescending(lambda)
        //            : query.OrderBy(lambda);
        //    }

        //    return query;
        //}

        ////alone method for creating paged result with filter expression and mapping function

        //public async Task<PagedResponse<TResult>> CreatePagedResultAsync<TEntity, TResult>(
        //    IQueryable<TEntity> query,
        //    PagedRequest request,
        //    Expression<Func<TEntity, bool>>? filterExpression = null,
        //    Func<TEntity, TResult> mappingFunction = null!)
        //    where TEntity : class
        //    where TResult : class
        //{
        //    // Apply filtering if provided
        //    if (filterExpression != null)
        //    {
        //        query = query.Where(filterExpression);
        //    }

        //    // Apply search if it's implemented in the request handler
        //    // Note: Specific search logic should be applied before calling this method

        //    // Get total count
        //    var totalCount = await query.CountAsync();

        //    // Apply sorting if requested
        //    if (!string.IsNullOrEmpty(request.SortColumn))
        //    {
        //        query = ApplySorting(query, request.SortColumn, request.SortDirection);
        //    }

        //    // Apply pagination
        //    var pagedItems = await query
        //        .Skip((request.PageNumber - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .AsNoTracking()
        //        .ToListAsync();

        //    // Map to DTO if a mapping function is provided
        //    var mappedItems = mappingFunction != null
        //        ? pagedItems.Select(mappingFunction).ToList()
        //        : pagedItems.Cast<TResult>().ToList();

        //    // Create response
        //    return new PagedResponse<TResult>
        //    {
        //        Items = mappedItems,
        //        TotalCount = totalCount,
        //        PageNumber = request.PageNumber,
        //        PageSize = request.PageSize
        //    };
        //}

        //private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortColumn, string? sortDirection)
        //{
        //    var entityType = typeof(T);
        //    var propertyName = sortColumn.First().ToString().ToUpper() + sortColumn[1..];

        //    var sortProperty = entityType.GetProperty(
        //        propertyName,
        //        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //    if (sortProperty == null)
        //    {
        //        return query; // Default ordering will be applied by the caller if needed
        //    }

        //    var parameter = Expression.Parameter(entityType, "x");
        //    var property = Expression.Property(parameter, sortProperty);
        //    var lambda = Expression.Lambda(property, parameter);

        //    var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        //    var orderByMethod = typeof(Queryable).GetMethods()
        //        .First(m => m.Name == methodName && m.GetParameters().Length == 2);
        //    var genericMethod = orderByMethod.MakeGenericMethod(entityType, sortProperty.PropertyType);

        //    return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda })!;
        //}

        public async Task<PagedResponse<TDto>> CreatePagedResponseAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            PagedRequest request,
            Func<TEntity, TDto> mapFunction,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            // Get total count first (before paging)
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(request.SortColumn))
                query = ApplySort(query, request.SortColumn, request.SortDirection);

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var mappedItems = items.Select(mapFunction).ToList();

            return new PagedResponse<TDto>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            string sortColumn,
            string? sortDirection = "asc")
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query;

            return query.ApplyDynamicOrderBy(sortColumn, sortDirection ?? "asc");
        }

        // Helper to combine search predicates with OR
        public Expression<Func<T, bool>> CombineSearchPredicates<T>(
            params Expression<Func<T, bool>>[] predicates)
        {
            if (predicates == null || predicates.Length == 0)
                return x => false;

            var result = predicates[0];

            for (int i = 1; i < predicates.Length; i++)
                result = PredicateBuilder.Or(result, predicates[i]);

            return result;
        }

    }
}