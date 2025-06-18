using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllUsersQuery, AppResult<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = request.ActiveOnly
                    ? await _unitOfWork.Users.GetActiveUsersAsync()
                    : await _unitOfWork.Users.GetAllAsync();

                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    // Get user roles
                    var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                    var allRoles = await _unitOfWork.Roles.GetAllAsync();

                    var rolesDto = userRoles
                        .Select(ur => allRoles.FirstOrDefault(r => r.Id == ur.RoleId))
                        .Where(r => r != null)
                        .Select(r => (RoleDto)r!)
                        .ToList();

                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = "", // Don't expose password hash
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        DateOfBirth = user.DateOfBirth,
                        Bio = user.Bio,
                        IsActive = user.IsActive,
                        CreatedOn = user.CreatedOn,
                        Roles = rolesDto
                    });
                }

                return AppResult<List<UserDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<UserDto>>.Failure($"An error occurred while retrieving users: {ex.Message}");
            }
        }
    }
}