// Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileValidator.cs
using FluentValidation;

namespace ECommercePlatform.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileValidator()
        {
            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName!)
                    .MaximumLength(50).WithMessage("First name must not exceed 50 characters");
            });

            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                RuleFor(x => x.LastName!)
                    .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
            });

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber!)
                    .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters");
            });

            When(x => !string.IsNullOrEmpty(x.Bio), () =>
            {
                RuleFor(x => x.Bio!)
                    .MaximumLength(500).WithMessage("Bio must not exceed 500 characters");
            });
        }
    }
}