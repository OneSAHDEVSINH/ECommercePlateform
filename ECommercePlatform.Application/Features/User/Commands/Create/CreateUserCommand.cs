using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Create
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}