namespace ECommercePlatform.Domain.Entities
{
    public class Module : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Route { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }

        // Updated navigation property
        public virtual ICollection<RolePermission>? RolePermissions { get; set; }

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