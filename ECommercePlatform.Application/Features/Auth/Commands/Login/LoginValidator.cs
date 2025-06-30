using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        private static readonly Regex EmailRegex = GeneratedRegex.Email();
        public LoginValidator()
        {
            RuleFor(x => x.Email.Trim())
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required")
                .Matches(EmailRegex)
                    .WithMessage("Email must be in a valid format (example@domain.com).");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}