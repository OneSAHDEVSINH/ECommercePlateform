using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Domain.Entities
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<RolePermission>? RolePermissions { get; set; }

        public Role() : base() { }
        public Role(string roleName) : base(roleName)
        {
        }

        public static Role Create(
            string name,
            string description)
        {
            return new Role
            {
                Name = name,
                Description = description
            };
        }

        public void Update(string name,
            string description)
        {
            Name = name;
            Description = description;
        }

        public void UpdateProperties(string? name = null, string? description = null)
        {
            if (name != null)
                Name = name;

            if (description != null)
                Description = description;
        }
    }
}