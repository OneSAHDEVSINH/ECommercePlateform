using System;
using System.Collections.Generic;
using System.Reflection;

namespace ECommercePlatform.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PermissionType Type { get; private set; }
        public Guid ModuleId { get; private set; }

        public Module Module { get; private set; }
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        public enum PermissionType
        {
            View,
            Create,
            Edit,
            Delete
        }

        public static Permission Create(
            string name,
            string description,
            PermissionType type,
            Guid moduleId,
            string createdBy)
        {
            return new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Type = type,
                ModuleId = moduleId,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                ModifiedBy = createdBy,
                ModifiedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };
        }
    }
}