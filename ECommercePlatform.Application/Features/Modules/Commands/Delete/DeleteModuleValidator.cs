using FluentValidation;

namespace ECommercePlatform.Application.Features.Modules.Commands.Delete
{
    public class DeleteModuleValidator : AbstractValidator<DeleteModuleCommand>
    {
        public DeleteModuleValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Module ID is required.");
        }
    }
}