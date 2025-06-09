using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetAllUsers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;

        public GetAllUsersHandler(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();
            var allUserRoles = new List<UserRole>();
            foreach (var user in users)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
                allUserRoles.AddRange(userRoles);
            }
            var allRoles = await _roleRepository.GetAllAsync();

            var result = new List<UserDto>();
            foreach (var user in users)
            {
                var userRoles = allUserRoles.Where(ur => ur.UserId == user.Id).ToList();
                var rolesDto = userRoles.Select(ur =>
                {
                    var role = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                    return new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive
                    };
                }).ToList();

                result.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Roles = rolesDto
                });
            }
            return result;
        }
    }
}