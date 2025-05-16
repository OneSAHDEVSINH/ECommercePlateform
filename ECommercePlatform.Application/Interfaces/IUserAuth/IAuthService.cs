using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IAuth
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
}