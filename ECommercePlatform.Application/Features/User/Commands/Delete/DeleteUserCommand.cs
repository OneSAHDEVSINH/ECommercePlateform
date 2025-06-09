using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Delete
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }
}