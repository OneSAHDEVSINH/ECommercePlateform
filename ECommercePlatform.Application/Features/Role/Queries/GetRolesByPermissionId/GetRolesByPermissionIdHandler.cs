using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRolesByPermissionId
{
    public class GetRolesByPermissionIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRolesByPermissionIdQuery, AppResult<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<RoleDto>>> Handle(GetRolesByPermissionIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify permission exists
                var permission = await _unitOfWork.Permissions.GetByIdAsync(request.PermissionId);
                if (permission == null)
                    return AppResult<List<RoleDto>>.Failure($"Permission with ID {request.PermissionId} not found.");

                var roles = await _unitOfWork.Roles.GetRolesByPermissionIdAsync(request.PermissionId);

                // Filter by active if requested
                if (request.ActiveOnly)
                    roles = roles.Where(r => r.IsActive && !r.IsDeleted).ToList();

                var roleDtos = new List<RoleDto>();

                foreach (var role in roles)
                {
                    var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(role.Id);
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

                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive,
                        //CreatedBy = role.CreatedBy,
                        CreatedOn = role.CreatedOn,
                        //ModifiedBy = role.ModifiedBy,
                        //ModifiedOn = role.ModifiedOn,
                        Permissions = permissionDtos
                    });
                }

                return AppResult<List<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<RoleDto>>.Failure($"An error occurred while retrieving roles by permission: {ex.Message}");
            }
        }
    }
}