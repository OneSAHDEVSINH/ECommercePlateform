using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Interfaces.IUserAuth
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
}