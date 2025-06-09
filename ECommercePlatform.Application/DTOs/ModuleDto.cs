namespace ECommercePlatform.Application.DTOs
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}
