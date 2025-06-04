using ECommercePlatform.Application.Models;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.Pagination
{
    /// <summary>
    /// Provides pagination capabilities to repositories
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IPaginationRepository<T> where T : class
    {
        /// <summary>
        /// Returns a queryable to build upon
        /// </summary>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// Retrieves a paged list with optional search, sorting and filtering
        /// </summary>
        Task<PagedResponse<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchText = null,
            string? sortColumn = null,
            string? sortDirection = null,
            Expression<Func<T, bool>>? filter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paged list using PagedRequest
        /// </summary>
        Task<PagedResponse<T>> GetPagedAsync(
            PagedRequest request,
            Expression<Func<T, bool>>? filter = null,
            CancellationToken cancellationToken = default);
    }
}