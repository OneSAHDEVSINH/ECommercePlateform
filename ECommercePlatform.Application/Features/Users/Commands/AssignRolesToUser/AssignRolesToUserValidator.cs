using FluentValidation;

namespace ECommercePlatform.Application.Features.Users.Commands.AssignRolesToUser
{
    public class AssignRolesToUserValidator : AbstractValidator<AssignRolesToUserCommand>
    {
        public AssignRolesToUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("Role IDs cannot be null")
                .Must(roles => roles.Count > 0)
                    .WithMessage("At least one role must be assigned");

            RuleForEach(x => x.RoleIds)
                .NotEmpty().WithMessage("Role ID cannot be empty");
        }
    }
}