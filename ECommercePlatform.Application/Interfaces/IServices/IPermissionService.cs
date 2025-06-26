using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IServices
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string moduleName, string permission);
        Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId);
        Task<bool> UserCanAccessModuleAsync(Guid userId, string moduleName);
        Task<List<string>> GetUserPermissionClaimsAsync(Guid userId);
    }
}
