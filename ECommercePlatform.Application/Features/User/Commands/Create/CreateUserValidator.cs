using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.User.Commands.Create
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        private static readonly Regex EmailRegex = GeneratedRegex.Email();
        private static readonly Regex NameRegex = GeneratedRegex.Name();

        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName.Trim())
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .Must(static value => NameRegex.IsMatch(value))
                    .WithMessage("First name must contain only letters and spaces");

            RuleFor(x => x.LastName.Trim())
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .Must(static value => NameRegex.IsMatch(value))
                    .WithMessage("Last name must contain only letters and spaces");

            RuleFor(x => x.Email.Trim())
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .EmailAddress().WithMessage("A valid email address is required")
                .Must(static value => EmailRegex.IsMatch(value))
                    .WithMessage("Please enter a valid email address format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("At least one role must be assigned")
                .Must(roles => roles != null && roles.Count > 0)
                    .WithMessage("At least one role must be assigned");

            RuleForEach(x => x.RoleIds)
                .NotEmpty().WithMessage("Role ID cannot be empty");
        }
    }
}