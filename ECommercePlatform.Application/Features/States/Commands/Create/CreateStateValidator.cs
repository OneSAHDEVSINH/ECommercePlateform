using FluentValidation;

namespace ECommercePlatform.Application.Features.States.Commands.Create
{
    public class CreateStateValidator : AbstractValidator<CreateStateCommand>
    {
        public CreateStateValidator()
        {
            RuleFor(x => x.Name)
            .NotNull().WithMessage("State Name is required !!!")
            .MaximumLength(100).WithMessage("State Name should not exeed 100 characters !!!")
            .Matches(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$").WithMessage("State name must have first letter capitalized for each word !!!");


            RuleFor(x => x.Code)
                .NotNull().WithMessage("State Code is required !!!")
                .MaximumLength(3).WithMessage("Code should not exceed 3 characters !!!")
                .Matches(@"^[A-Z]{1,3}$").WithMessage("Code must be 1 to 3 uppercase letters !!!");

        }
    }
}
