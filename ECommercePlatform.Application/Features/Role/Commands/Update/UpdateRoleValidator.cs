using FluentValidation;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(250);
            RuleFor(x => x.Permissions).NotNull();
            RuleForEach(x => x.Permissions).SetValidator(new UpdateRolePermissionDtoValidator());
        }
    }

    public class UpdateRolePermissionDtoValidator : AbstractValidator<UpdateRolePermissionDto>
    {
        public UpdateRolePermissionDtoValidator()
        {
            RuleFor(x => x.ModuleId).NotEmpty();
            RuleFor(x => x.PermissionType).NotEmpty();
        }
    }
}