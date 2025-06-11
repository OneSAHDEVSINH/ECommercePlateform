using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;

namespace ECommercePlatform.Application.DTOs
{
    public class PermissionDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public PermissionType Type { get; init; }
        public Guid ModuleId { get; init; }
        public string? ModuleName { get; init; }
        public string? ModuleRoute { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }

        // Explicit conversion operator from Permission to PermissionDto
        public static explicit operator PermissionDto(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Type = permission.Type,
                ModuleId = permission.ModuleId,
                ModuleName = permission.Module?.Name,
                ModuleRoute = permission.Module?.Route,
                IsActive = permission.IsActive,
                CreatedOn = permission.CreatedOn
            };
        }
    }

    public class CreatePermissionDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public PermissionType Type { get; init; }
        public required Guid ModuleId { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public class UpdatePermissionDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public PermissionType? Type { get; init; }
        public Guid? ModuleId { get; init; }
        public bool? IsActive { get; init; }

        //public static explicit operator UpdatePermissionDto(UpdatePermissionCommand command)
        //{
        //    return new UpdatePermissionDto
        //    {
        //        Name = command.Name,
        //        Description = command.Description,
        //        Type = command.Type,
        //        ModuleId = command.ModuleId,
        //        IsActive = command.IsActive
        //    };
        //}
    }

    public class PermissionListDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public PermissionType Type { get; init; }
        public string? ModuleName { get; init; }
        public bool IsActive { get; init; }

        public static explicit operator PermissionListDto(Permission permission)
        {
            return new PermissionListDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Type = permission.Type,
                ModuleName = permission.Module?.Name,
                IsActive = permission.IsActive
            };
        }
    }
}