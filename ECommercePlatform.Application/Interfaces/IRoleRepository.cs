using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        new Task<Role?> GetByIdAsync(Guid id);
        new Task<List<Role>> GetAllAsync();
        Task<List<Role>> GetActiveRolesAsync();
        IQueryable<Role> AsQueryable();
        Task<Role?> GetRoleWithPermissionsAsync(Guid id);
        Task<Result<string>> EnsureNameIsUniqueAsync(string name, Guid? excludeId = null);

        // Pagination methods
        Task<PagedResponse<Role>> GetPagedRolesAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<RoleDto>> GetPagedRoleDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<RoleListDto>> GetPagedRoleListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}