using FluentValidation;

namespace ECommercePlatform.Application.Features.Role.Commands.AssignPermissions
{
    public class AssignPermissionsToRoleValidator : AbstractValidator<AssignPermissionsToRoleCommand>
    {
        public AssignPermissionsToRoleValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required.");

            RuleFor(x => x.Permissions)
                .NotNull().WithMessage("Permissions collection cannot be null.");

            RuleForEach(x => x.Permissions)
                .ChildRules(permission => {
                    permission.RuleFor(p => p.ModuleId)
                        .NotEmpty().WithMessage("Module ID is required for permission.");

                    permission.RuleFor(p => p.PermissionTypes)
                        .NotNull().WithMessage("Permission types collection cannot be null.");

                    permission.RuleForEach(p => p.PermissionTypes)
                        .NotEmpty().WithMessage("Permission type cannot be empty.")
                        .Must(BeValidPermissionType).WithMessage("Permission type must be one of: View, Add, Edit, Delete.");
                });
        }

        private bool BeValidPermissionType(string permissionType)
        {
            return permissionType is "View" or "Add" or "Edit" or "Delete";
        }
    }
}