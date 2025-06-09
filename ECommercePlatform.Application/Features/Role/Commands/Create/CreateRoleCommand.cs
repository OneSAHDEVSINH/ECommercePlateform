using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<CreateRolePermissionDto> Permissions { get; set; } = new List<CreateRolePermissionDto>();
    }

    public class CreateRolePermissionDto
    {
        public Guid ModuleId { get; set; }
        public string PermissionType { get; set; } // View, Create, Edit, Delete
    }
}