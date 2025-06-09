namespace ECommercePlatform.Application.DTOs
{
    public class RolePermissionDto
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionType { get; set; } // View, Create, Edit, Delete
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }
    }
}
