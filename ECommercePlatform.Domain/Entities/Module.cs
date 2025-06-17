namespace ECommercePlatform.Domain.Entities
{
    public class Module : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public string? Route { get; private set; }
        public string? Icon { get; private set; }
        public int DisplayOrder { get; private set; }

        // Updated navigation property
        public virtual ICollection<RolePermission>? RolePermissions { get; private set; }

        private Module() { }

        public static Module Create(
            string name,
            string description,
            string route,
            string icon,
            int displayOrder)
        {
            return new Module
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Route = route,
                Icon = icon,
                DisplayOrder = displayOrder
            };
        }

        public void Update(string name,
            string description,
            string route,
            string icon,
            int displayOrder)
        {
            Name = name;
            Description = description;
            Route = route;
            Icon = icon;
            DisplayOrder = displayOrder;
            ModifiedOn = DateTime.Now;
        }
    }
}