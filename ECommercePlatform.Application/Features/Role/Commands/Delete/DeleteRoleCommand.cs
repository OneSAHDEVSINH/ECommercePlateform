using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Delete
{
    public class DeleteRoleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteRoleCommand(Guid id)
        {
            Id = id;
        }
    }
}