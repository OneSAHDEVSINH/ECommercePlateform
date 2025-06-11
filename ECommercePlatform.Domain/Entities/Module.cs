namespace ECommercePlatform.Domain.Entities
{
    public class Module : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public string? Route { get; private set; }
        public string? Icon { get; private set; }
        public int DisplayOrder { get; private set; }

        public ICollection<Permission>? Permissions { get; set; }

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
        }
    }
}