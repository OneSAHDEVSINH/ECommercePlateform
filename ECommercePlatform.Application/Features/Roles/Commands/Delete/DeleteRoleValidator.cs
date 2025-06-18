using FluentValidation;

namespace ECommercePlatform.Application.Features.Roles.Commands.Delete
{
    public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}