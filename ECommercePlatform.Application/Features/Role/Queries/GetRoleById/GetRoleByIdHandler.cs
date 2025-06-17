using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleById
{
    public class GetRoleByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoleByIdQuery, AppResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                    return AppResult<RoleDto>.Failure($"Role with ID {request.Id} not found.");

                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(role.Id);
                var allPermissions = await _unitOfWork.Permissions.GetAllAsync();

                var permissionsDto = rolePermissions.Select(rp =>
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Id == rp.PermissionId);
                    return new RolePermissionDto
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        PermissionType = perm?.Type ?? PermissionType.View,
                        ModuleId = perm?.ModuleId ?? Guid.Empty,
                        ModuleName = perm?.Module?.Name ?? string.Empty
                    };
                }).ToList();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    //CreatedBy = role.CreatedBy,
                    CreatedOn = role.CreatedOn,
                    //ModifiedBy = role.ModifiedBy,
                    //ModifiedOn = role.ModifiedOn,
                    Permissions = permissionsDto
                };

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}