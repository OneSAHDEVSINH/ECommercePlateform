using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Create
{
    public class CreateUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Users.EnsureEmailIsUniqueAsync(request.Email)
                    .Map(email =>
                    {
                        var user = new Domain.Entities.User();
                        user = user.With(
                            firstName: request.FirstName,
                            lastName: request.LastName,
                            email: email,
                            password: request.Password,
                            gender: request.Gender,
                            dateOfBirth: request.DateOfBirth,
                            phoneNumber: request.PhoneNumber,
                            bio: request.Bio
                        );
                        user.IsActive = request.IsActive;
                        user.CreatedBy = request.CreatedBy;
                        user.CreatedOn = request.CreatedOn;
                        return user;
                    })
                    .Tap(async user => await _unitOfWork.Users.AddAsync(user))
                    .Tap(async user =>
                    {
                        if (request.RoleIds != null && request.RoleIds.Any())
                        {
                            foreach (var roleId in request.RoleIds)
                            {
                                var userRole = Domain.Entities.UserRole.Create(user.Id, roleId);
                                userRole.CreatedBy = request.CreatedBy;
                                userRole.CreatedOn = request.CreatedOn;
                                await _unitOfWork.UserRoles.AddAsync(userRole);
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();
                    })
                    .Map(user => AppResult<UserDto>.Success((UserDto)user));

                return result.IsSuccess
                    ? result.Value
                    : AppResult<UserDto>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return AppResult<UserDto>.Failure($"An error occurred while creating the user: {ex.Message}");
            }
        }
    }
}