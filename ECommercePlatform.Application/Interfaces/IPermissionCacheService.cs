using ECommercePlatform.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IPermissionCacheService
    {
        Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId);
        Task InvalidateUserPermissionsAsync(Guid userId);
        Task<bool> HasPermissionAsync(Guid userId, string module, string permission);
        Task InvalidateAllPermissionsAsync();
    }
}
