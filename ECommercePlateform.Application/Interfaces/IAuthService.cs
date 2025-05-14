using ECommercePlateform.Application.DTOs;

namespace ECommercePlateform.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
}