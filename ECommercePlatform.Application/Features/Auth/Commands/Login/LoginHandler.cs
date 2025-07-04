﻿using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Auth.Commands.Login
{
    public class LoginHandler(IAuthService authService) : IRequestHandler<LoginCommand, AppResult<AuthResultDto>>
    {
        private readonly IAuthService _authService = authService;

        public async Task<AppResult<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return AppResult<AuthResultDto>.Failure("Email and password are required");


                var loginDto = new LoginDto
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _authService.LoginAsync(loginDto);

                return result.IsSuccess
                    ? AppResult<AuthResultDto>.Success(result.Value)
                    : AppResult<AuthResultDto>.Failure(result.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AppResult<AuthResultDto>.Failure(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return AppResult<AuthResultDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex}");
                return AppResult<AuthResultDto>.Failure($"An error occurred during login: {ex.Message}");
            }
        }
    }
}

//if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
//    return AppResult<AuthResultDto>.Failure("Email and password are required");

//var loginDto = new LoginDto
//{
//    Email = request.Email,
//    Password = request.Password
//};

//var result = await _authService.LoginAsync(loginDto);

//return AppResult<AuthResultDto>.Success(result);