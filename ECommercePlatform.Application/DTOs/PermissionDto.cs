namespace ECommercePlatform.Application.DTOs
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; } // View, Create, Edit, Delete
        public Guid ModuleId { get; set; }
        public string? ModuleName { get; set; }
    }
}
