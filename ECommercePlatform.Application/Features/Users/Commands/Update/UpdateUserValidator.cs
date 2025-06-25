using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Users.Commands.Update
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        private static readonly Regex EmailRegex = GeneratedRegex.Email();
        private static readonly Regex NameRegex = GeneratedRegex.Name();

        public UpdateUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required");

            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName!.Trim())
                    .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                    .Must(static value => NameRegex.IsMatch(value))
                        .WithMessage("First name must contain only letters and spaces");
            });

            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                RuleFor(x => x.LastName!.Trim())
                    .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                    .Must(static value => NameRegex.IsMatch(value))
                        .WithMessage("Last name must contain only letters and spaces");
            });

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email!.Trim())
                    .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                    .EmailAddress().WithMessage("A valid email address is required")
                    .Must(static value => EmailRegex.IsMatch(value))
                        .WithMessage("Please enter a valid email address format");
            });

            When(x => !string.IsNullOrEmpty(x.Password), () =>
            {
                RuleFor(x => x.Password!)
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                    .MaximumLength(100).WithMessage("Password must not exceed 100 characters");
            });

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber!.Trim())
                    .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters");
            });

            When(x => !string.IsNullOrEmpty(x.Bio), () =>
            {
                RuleFor(x => x.Bio!)
                    .MaximumLength(500).WithMessage("Bio must not exceed 500 characters");
            });

            When(x => x.RoleIds != null && x.RoleIds.Any(), () =>
            {
                RuleForEach(x => x.RoleIds!)
                    .NotEmpty().WithMessage("Role ID cannot be empty");
            });
        }
    }
}