using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IUserAuth
{
    public interface IAuthService
    {
        //Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AppResult<AuthResultDto>> LoginAsync(LoginDto loginDto);
        Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId);
    }
}