using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.DTOs
{
    public class RolePermissionDto
    {
        public Guid Id { get; init; }
        public Guid RoleId { get; init; }
        public Guid ModuleId { get; init; }
        public string? RoleName { get; init; }
        public string? ModuleName { get; init; }
        public bool CanView { get; init; }
        //public bool CanAdd { get; init; }
        //public bool CanEdit { get; init; }
        public bool CanAddEdit { get; init; }
        public bool CanDelete { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }

        public static explicit operator RolePermissionDto(RolePermission rolePermission)
        {
            return new RolePermissionDto
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                ModuleId = rolePermission.ModuleId,
                RoleName = rolePermission.Role?.Name,
                ModuleName = rolePermission.Module?.Name,
                CanView = rolePermission.CanView,
                //CanAdd = rolePermission.CanAdd,
                //CanEdit = rolePermission.CanEdit,
                CanAddEdit = rolePermission.CanAddEdit,
                CanDelete = rolePermission.CanDelete,
                IsActive = rolePermission.IsActive,
                CreatedOn = rolePermission.CreatedOn
            };
        }
    }

    public class CreateRolePermissionDto
    {
        public required Guid RoleId { get; init; }
        public required Guid ModuleId { get; init; }
        public bool CanView { get; init; }
        //public bool CanAdd { get; init; }
        //public bool CanEdit { get; init; }
        public bool CanAddEdit { get; init; }
        public bool CanDelete { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public class UpdateRolePermissionDto
    {
        public bool CanView { get; init; }
        //public bool CanAdd { get; init; }
        //public bool CanEdit { get; init; }
        public bool CanAddEdit { get; init; }
        public bool CanDelete { get; init; }
        public bool? IsActive { get; init; }
    }

    public class UpdateRolePermissionCommand : IRequest<AppResult<RolePermissionDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public bool CanView { get; init; }
        //public bool CanAdd { get; init; }
        //public bool CanEdit { get; init; }
        public bool CanAddEdit { get; init; }
        public bool CanDelete { get; init; }
        public bool? IsActive { get; init; }

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}