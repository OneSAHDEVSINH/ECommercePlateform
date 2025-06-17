using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, AppResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate role name uniqueness
                var nameResult = await _unitOfWork.Roles.EnsureNameIsUniqueAsync(request.Name);
                if (nameResult.IsFailure)
                    return AppResult<RoleDto>.Failure(nameResult.Error);

                // Create the role entity
                var role = Domain.Entities.Role.Create(request.Name, request.Description ?? string.Empty);
                role.IsActive = request.IsActive;
                role.CreatedBy = request.CreatedBy;
                role.CreatedOn = request.CreatedOn;

                await _unitOfWork.Roles.AddAsync(role);

                // Process permissions
                foreach (var perm in request.Permissions)
                {
                    // Parse permission type from string to enum
                    if (!Enum.TryParse<PermissionType>(perm.PermissionType, true, out var permissionTypeEnum))
                        continue; // Skip invalid permission types

                    var permission = await _unitOfWork.Permissions.GetByModuleAndTypeAsync(perm.ModuleId, perm.PermissionType);
                    if (permission != null)
                    {
                        var rolePermission = RolePermission.Create(role.Id, permission.Id);
                        rolePermission.CreatedBy = request.CreatedBy;
                        rolePermission.CreatedOn = request.CreatedOn;
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload the role with permissions for return
                var createdRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                if (createdRole == null)
                    return AppResult<RoleDto>.Failure("Role was created but could not be retrieved.");

                // Map Entity to DTO
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(createdRole.Id);
                var allPermissions = await _unitOfWork.Permissions.GetAllAsync();

                var permissionDtos = rolePermissions.Select(rp =>
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
                    Id = createdRole.Id,
                    Name = createdRole.Name,
                    Description = createdRole.Description,
                    IsActive = createdRole.IsActive,
                    CreatedOn = createdRole.CreatedOn,
                    //CreatedBy = createdRole.CreatedBy,
                    Permissions = permissionDtos
                };

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred while creating the role: {ex.Message}");
            }
        }
    }
}