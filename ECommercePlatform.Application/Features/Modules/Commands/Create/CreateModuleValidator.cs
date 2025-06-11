using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Modules.Commands.Create
{
    public class CreateModuleValidator : AbstractValidator<CreateModuleCommand>
    {
        private static readonly Regex RouteRegex = GeneratedRegex.RouteFormat();
        private static readonly Regex NameRegex = GeneratedRegex.AlphanumericWithSpaces();

        public CreateModuleValidator()
        {
            RuleFor(x => x.Name.Trim())
                .NotEmpty().WithMessage("Module name is required.")
                .MaximumLength(100).WithMessage("Module name must not exceed 100 characters.")
                .Must(name => NameRegex.IsMatch(name))
                    .WithMessage("Module name must contain only letters, numbers and spaces.");

            RuleFor(x => x.Route.Trim())
                .NotEmpty().WithMessage("Route is required.")
                .MaximumLength(50).WithMessage("Route must not exceed 50 characters.")
                .Must(route => RouteRegex.IsMatch(route))
                    .WithMessage("Route must be URL-friendly (lowercase letters, numbers, and hyphens only).");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Icon)
                .MaximumLength(100).WithMessage("Icon class must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Icon));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative number.");
        }
    }
}