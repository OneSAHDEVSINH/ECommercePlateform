using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<UpdateRolePermissionDto> Permissions { get; set; }
    }

    public class UpdateRolePermissionDto
    {
        public Guid ModuleId { get; set; }
        public string PermissionType { get; set; } // View, Create, Edit, Delete
    }
}