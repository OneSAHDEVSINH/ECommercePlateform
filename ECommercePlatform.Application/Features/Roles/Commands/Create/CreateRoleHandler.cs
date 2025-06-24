using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Commands.Create
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
                var role = Role.Create(
                    Guid.NewGuid(),
                    request.Name,
                    request.Description ?? string.Empty,
                    request.CreatedBy ?? "system");

                role.IsActive = request.IsActive;

                await _unitOfWork.Roles.AddAsync(role);

                // Process permissions if provided
                if (request.Permissions != null && request.Permissions.Any())
                {
                    foreach (var perm in request.Permissions)
                    {
                        // Verify module exists
                        var module = await _unitOfWork.Modules.GetByIdAsync(perm.ModuleId);
                        if (module == null) continue;

                        var rolePermission = RolePermission.Create(
                            role.Id,
                            perm.ModuleId,
                            perm.CanView,
                            //perm.CanAdd,
                            //perm.CanEdit,
                            perm.CanAddEdit,
                            perm.CanDelete
                        );

                        rolePermission.SetCreatedBy(request.CreatedBy ?? "system");
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload the role with permissions for return
                var createdRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                if (createdRole == null)
                    return AppResult<RoleDto>.Failure("Role was created but could not be retrieved.");

                // Map to DTO using explicit operator
                var roleDto = (RoleDto)createdRole;

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred while creating the role: {ex.Message}");
            }
        }
    }
}