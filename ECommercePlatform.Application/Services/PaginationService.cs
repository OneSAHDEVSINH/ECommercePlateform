using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Common.Helpers;
using ECommercePlatform.Application.Interfaces.Pagination;
using ECommercePlatform.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Services
{
    public class PaginationService : IPaginationService
    {
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