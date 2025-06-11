using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        private static readonly Regex NameRegex = GeneratedRegex.AlphanumericWithSpaces();

        public CreateRoleValidator()
        {
            RuleFor(x => x.Name.Trim())
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.")
                .Must(name => NameRegex.IsMatch(name))
                    .WithMessage("Role name must contain only letters, numbers, and spaces.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleForEach(x => x.Permissions)
                .ChildRules(permission => {
                    permission.RuleFor(p => p.ModuleId)
                        .NotEmpty().WithMessage("Module ID is required for permission.");

                    permission.RuleFor(p => p.PermissionType)
                        .NotEmpty().WithMessage("Permission type is required.")
                        .Must(BeValidPermissionType).WithMessage("Permission type must be one of: VIEW, ADD, EDIT, DELETE.");
                });
        }

        private bool BeValidPermissionType(string permissionType)
        {
            return permissionType is "VIEW" or "ADD" or "EDIT" or "DELETE";
        }
    }
}