//using ECommercePlatform.Application.Features.Role.Commands.Update;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }
        public List<RoleModulePermissionDto>? Permissions { get; init; }

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
                    .GroupBy(rp => new { rp.ModuleId, rp.Module?.Name })
                    .Select(g => new RoleModulePermissionDto
                    {
                        ModuleId = g.Key.ModuleId,
                        ModuleName = g.Key.Name,
                        CanView = g.First().CanView,
                        CanAdd = g.First().CanAdd,
                        CanEdit = g.First().CanEdit,
                        CanDelete = g.First().CanDelete
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
        public List<RoleModulePermissionDto>? Permissions { get; init; }
    }

    public class UpdateRoleDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; } = true;
        public List<RoleModulePermissionDto>? Permissions { get; init; }

        //public static explicit operator UpdateRoleDto(UpdateRoleCommand command)
        //{
        //    return new UpdateRoleDto
        //    {
        //        Name = command.Name,
        //        Description = command.Description,
        //        IsActive = (bool)command.IsActive,
        //        Permissions = command.Permissions?.Select(p => new ModulePermissionRequest
        //        {
        //            ModuleId = p.ModuleId,
        //            PermissionTypes = new List<string> { p.PermissionType }
        //        }).ToList()
        //    };
        //}
    }

    public class RoleModulePermissionDto
    {
        public Guid ModuleId { get; init; }
        public string? ModuleName { get; init; }
        public bool CanView { get; init; }
        public bool CanAdd { get; init; }
        public bool CanEdit { get; init; }
        public bool CanDelete { get; init; }
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
                PermissionCount = role.RolePermissions?
                    .Where(rp => rp.CanView || rp.CanAdd || rp.CanEdit || rp.CanDelete)
                    .Count() ?? 0,
                CreatedOn = role.CreatedOn
            };
        }
    }
}