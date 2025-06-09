using FluentValidation;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(250);
            RuleFor(x => x.Permissions).NotNull();
            RuleForEach(x => x.Permissions).SetValidator(new CreateRolePermissionDtoValidator());
        }
    }

    public class CreateRolePermissionDtoValidator : AbstractValidator<CreateRolePermissionDto>
    {
        public CreateRolePermissionDtoValidator()
        {
            RuleFor(x => x.ModuleId).NotEmpty();
            RuleFor(x => x.PermissionType).NotEmpty();
        }
    }
}