using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
                    return AppResult<UserDto>.Failure("User is not authenticated");

                var userId = Guid.Parse(_currentUserService.UserId); // Convert string UserId to Guid
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                    return AppResult<UserDto>.Failure("User not found");

                var userDto = new UserDto
                {
                    Id = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive
                };

                return AppResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return AppResult<UserDto>.Failure($"An error occurred while getting the current user: {ex.Message}");
            }
        }
    }
}
