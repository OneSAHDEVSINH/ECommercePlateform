using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces.IRepositories
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task<List<UserRole>> GetByUserIdAsync(Guid userId);
        Task<List<UserRole>> GetByRoleIdAsync(Guid roleId);
        Task DeleteByUserIdAsync(Guid userId);

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