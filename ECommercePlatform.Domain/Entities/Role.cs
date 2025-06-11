namespace ECommercePlatform.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }

        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }

        private Role() { }

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