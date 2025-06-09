namespace ECommercePlatform.Domain.Entities
{
    public class Module : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public string? Route { get; private set; }
        public string? Icon { get; private set; }
        public int DisplayOrder { get; private set; }

        public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();

        public static Module Create(
            string name,
            string description,
            string route,
            string icon,
            int displayOrder,
            string createdBy)
        {
            return new Module
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Route = route,
                Icon = icon,
                DisplayOrder = displayOrder,
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