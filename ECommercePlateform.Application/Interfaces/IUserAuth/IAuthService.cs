using ECommercePlateform.Application.DTOs;

namespace ECommercePlateform.Application.Interfaces.IAuth
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
}