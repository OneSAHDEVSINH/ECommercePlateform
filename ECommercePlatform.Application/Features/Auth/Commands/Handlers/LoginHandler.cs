using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Auth.Commands.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, AppResult<AuthResultDto>>
    {
        private readonly IAuthService _authService;

        public LoginHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AppResult<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return AppResult<AuthResultDto>.Failure("Email and password are required");
                }

                var loginDto = new LoginDto
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _authService.LoginAsync(loginDto);

                return AppResult<AuthResultDto>.Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                return AppResult<AuthResultDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return AppResult<AuthResultDto>.Failure($"An error occurred during login: {ex.Message}");
            }
        }
    }
}
