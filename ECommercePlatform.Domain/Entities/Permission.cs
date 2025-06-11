using ECommercePlatform.Domain.Enums;

namespace ECommercePlatform.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public PermissionType Type { get; private set; }
        public Guid ModuleId { get; private set; }

        public Module? Module { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }

        private Permission() { }

        public static Permission Create(
            string name,
            string description,
            PermissionType type,
            Guid moduleId)
        {
            return new Permission
            {
                Name = name,
                Description = description,
                Type = type,
                ModuleId = moduleId
            };
        }

        public void Update(string name,
            string description,
            PermissionType type,
            Guid moduleId)
        {
            Name = name;
            Description = description;
            Type = type;
            ModuleId = moduleId;
        }
    }
}