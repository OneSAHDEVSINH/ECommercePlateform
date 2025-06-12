//using ECommercePlatform.Application.Features.Role.Commands.Update;
using ECommercePlatform.Application.Features.Role.Commands.Update;
using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommercePlatform.Application.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }
        public List<RolePermissionDto>? Permissions { get; init; }

        // Explicit conversion operator from Role to RoleDto
        public static explicit operator RoleDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedOn = role.CreatedOn,
                Permissions = role.RolePermissions?
                    .Where(rp => rp != null && rp.Permission != null)
                    .Select(rp => new RolePermissionDto
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        RoleName = role.Name,
                        //PermissionName = rp.Permission?.Name,
                        PermissionType = rp.Permission?.Type ?? PermissionType.View,
                        ModuleId = rp.Permission?.ModuleId ?? Guid.Empty,
                        ModuleName = rp.Permission?.Module?.Name,
                        ModuleRoute = rp.Permission?.Module?.Route,
                        IsActive = rp.IsActive,
                        CreatedOn = rp.CreatedOn
                    })
                    .ToList()
            };
        }
    }

    public class CreateRoleDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; } = true;
        public List<ModulePermissionRequest>? Permissions { get; init; }
    }

    public class UpdateRoleDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; } = true;
        public List<ModulePermissionRequest>? Permissions { get; init; }

        public static explicit operator UpdateRoleDto(UpdateRoleCommand command)
        {
            return new UpdateRoleDto
            {
                Name = command.Name,
                Description = command.Description,
                IsActive = (bool)command.IsActive,
                Permissions = command.Permissions?.Select(p => new ModulePermissionRequest
                {
                    ModuleId = p.ModuleId,
                    PermissionTypes = new List<string> { p.PermissionType }
                }).ToList()
            };
        }
    }

    public class ModulePermissionRequest
    {
        public Guid ModuleId { get; init; }
        public List<string> PermissionTypes { get; init; } = new();
    }

    public class RoleListDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public int PermissionCount { get; init; }
        public DateTime CreatedOn { get; init; }

        public static explicit operator RoleListDto(Role role)
        {
            return new RoleListDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                PermissionCount = role.RolePermissions?.Count ?? 0,
                CreatedOn = role.CreatedOn
            };
        }
    }
}