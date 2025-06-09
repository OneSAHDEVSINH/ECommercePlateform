using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task DeleteByRoleIdAsync(Guid roleId);
    }
}
