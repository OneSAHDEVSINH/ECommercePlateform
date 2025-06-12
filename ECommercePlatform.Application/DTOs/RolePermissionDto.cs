using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.DTOs
{
    public class RolePermissionDto
    {
        public Guid Id { get; init; }
        public Guid RoleId { get; init; }
        public Guid PermissionId { get; init; }
        public string? RoleName { get; init; }
        public string? PermissionName { get; init; }
        public PermissionType PermissionType { get; init; }
        public Guid ModuleId { get; init; }
        public string? ModuleName { get; init; }
        public string? ModuleRoute { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }

        // Explicit conversion operator from RolePermission to RolePermissionDto
        public static explicit operator RolePermissionDto(RolePermission rolePermission)
        {
            return new RolePermissionDto
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId,
                RoleName = rolePermission.Role?.Name,
                //PermissionName = rolePermission.Permission?.Name,
                PermissionType = rolePermission.Permission?.Type ?? PermissionType.View,
                ModuleId = rolePermission.Permission?.ModuleId ?? Guid.Empty,
                ModuleName = rolePermission.Permission?.Module?.Name,
                ModuleRoute = rolePermission.Permission?.Module?.Route,
                IsActive = rolePermission.IsActive,
                CreatedOn = rolePermission.CreatedOn
            };
        }
    }

    public class CreateRolePermissionDto
    {
        public required Guid RoleId { get; init; }
        public required Guid PermissionId { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public class UpdateRolePermissionDto
    {
        public Guid? RoleId { get; init; }
        public Guid? PermissionId { get; init; }
        public bool? IsActive { get; init; }

        public static explicit operator UpdateRolePermissionDto(UpdateRolePermissionCommand command)
        {
            return new UpdateRolePermissionDto
            {
                RoleId = command.RoleId,
                PermissionId = command.PermissionId,
                IsActive = command.IsActive
            };
        }
    }

    public class RolePermissionListDto
    {
        public Guid Id { get; init; }
        public Guid RoleId { get; init; }
        public Guid PermissionId { get; init; }
        public string? RoleName { get; init; }
        public string? PermissionName { get; init; }
        public PermissionType PermissionType { get; init; }
        public string? ModuleName { get; init; }
        public bool IsActive { get; init; }

        public static explicit operator RolePermissionListDto(RolePermission rolePermission)
        {
            return new RolePermissionListDto
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId,
                RoleName = rolePermission.Role?.Name,
                //PermissionName = rolePermission.Permission?.Name,
                PermissionType = rolePermission.Permission?.Type ?? PermissionType.View,
                ModuleName = rolePermission.Permission?.Module?.Name,
                IsActive = rolePermission.IsActive
            };
        }
    }

    public class RolePermissionRequest
    {
        public required Guid RoleId { get; init; }
        public required List<ModulePermissionsDto> Permissions { get; init; } = new();
    }

    public class ModulePermissionsDto
    {
        public required Guid ModuleId { get; init; }
        public required List<PermissionType> PermissionTypes { get; init; } = new();
    }

    public class UpdateRolePermissionCommand : IRequest<AppResult<RolePermissionDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public Guid? RoleId { get; init; }
        public Guid? PermissionId { get; init; }
        public bool? IsActive { get; init; }

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}