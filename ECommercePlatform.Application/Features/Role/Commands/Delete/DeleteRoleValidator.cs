using FluentValidation;

namespace ECommercePlatform.Application.Features.Role.Commands.Delete
{
    public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}