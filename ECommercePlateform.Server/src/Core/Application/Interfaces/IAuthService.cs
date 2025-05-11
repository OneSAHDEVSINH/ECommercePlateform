using ECommercePlateform.Server.Core.Application.DTOs;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Core.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    }
} 