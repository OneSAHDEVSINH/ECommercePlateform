using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
        Task DeleteByRoleIdAsync(Guid roleId);
        Task<bool> ExistsAsync(Guid roleId, Guid permissionId);
        Task<List<RolePermission>> GetByRoleIdAndModuleIdAsync(Guid roleId, Guid moduleId);
        Task<bool> AnyAsync(Expression<Func<RolePermission, bool>> predicate);
        IQueryable<RolePermission> AsQueryable();
        Task<List<RolePermission>> GetActiveRolePermissionsAsync();

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