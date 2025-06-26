using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IRepositories
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<List<RolePermission>> GetByModuleIdAsync(Guid moduleId);
        Task DeleteByRoleIdAsync(Guid roleId);
        Task<bool> AnyAsync(Expression<Func<RolePermission, bool>> predicate);
        IQueryable<RolePermission> AsQueryable();

        // Pagination methods
        Task<PagedResponse<RolePermission>> GetPagedRolePermissionsAsync(
            PagedRequest request,
            Guid? roleId = null,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<RolePermissionDto>> GetPagedRolePermissionDtosAsync(
            PagedRequest request,
            Guid? roleId = null,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}