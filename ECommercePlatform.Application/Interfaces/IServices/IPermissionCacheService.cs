using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IServices
{
    public interface IPermissionCacheService
    {
        Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId);
        Task InvalidateUserPermissionsAsync(Guid userId);
        Task<bool> HasPermissionAsync(Guid userId, string module, string permission);
        Task InvalidateAllPermissionsAsync();
    }
}
