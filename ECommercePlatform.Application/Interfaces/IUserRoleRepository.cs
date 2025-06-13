using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task<List<UserRole>> GetByUserIdAsync(Guid userId);
        Task<List<UserRole>> GetByRoleIdAsync(Guid roleId);
        Task DeleteByUserIdAsync(Guid userId);
        Task DeleteByRoleIdAsync(Guid roleId);
        Task<bool> ExistsAsync(Guid userId, Guid roleId);
        Task<bool> AnyAsync(Expression<Func<UserRole, bool>> predicate);
        IQueryable<UserRole> AsQueryable();
        Task<List<UserRole>> GetActiveUserRolesAsync();

        // Pagination methods
        Task<PagedResponse<UserRole>> GetPagedUserRolesAsync(
            PagedRequest request,
            Guid? userId = null,
            Guid? roleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<UserRoleDto>> GetPagedUserRoleDtosAsync(
            PagedRequest request,
            Guid? userId = null,
            Guid? roleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}