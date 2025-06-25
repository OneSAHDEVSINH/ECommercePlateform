using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Modules.Commands.Update
{
    public class UpdateModuleValidator : AbstractValidator<UpdateModuleCommand>
    {
        private static readonly Regex RouteRegex = GeneratedRegex.RouteFormat();
        private static readonly Regex NameRegex = GeneratedRegex.AlphanumericWithSpaces();

        public UpdateModuleValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Module ID is required.");

            When(x => !string.IsNullOrEmpty(x.Name), () =>
            {
                RuleFor(x => x.Name!.Trim())
                    .MaximumLength(100).WithMessage("Module name must not exceed 100 characters.")
                    .Must(name => NameRegex.IsMatch(name))
                        .WithMessage("Module name must contain only letters, numbers, and spaces.");
            });

            When(x => !string.IsNullOrEmpty(x.Route), () =>
            {
                RuleFor(x => x.Route!.Trim())
                    .MaximumLength(50).WithMessage("Route must not exceed 50 characters.")
                    .Must(route => RouteRegex.IsMatch(route))
                        .WithMessage("Route must be URL-friendly (lowercase letters, numbers, and hyphens only).");
            });

            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description!.Trim())
                    .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
            });

            When(x => !string.IsNullOrEmpty(x.Icon!.Trim()), () =>
            {
                RuleFor(x => x.Icon)
                    .MaximumLength(100).WithMessage("Icon class must not exceed 100 characters.");
            });

            When(x => x.DisplayOrder.HasValue, () =>
            {
                RuleFor(x => x.DisplayOrder!.Value)
                    .GreaterThanOrEqualTo(1).WithMessage("Display order must be a non-negative number except zero.");
            });
        }
    }
}