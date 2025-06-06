using ECommercePlatform.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ECommercePlatform.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
        
        public static Role Create(
            string name,
            string description,
            string createdBy)
        {
            return new Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now,
                ModifiedBy = createdBy,
                ModifiedOn = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
        }
    }
}