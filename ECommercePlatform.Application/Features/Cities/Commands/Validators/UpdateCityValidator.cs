using FluentValidation;

namespace ECommercePlatform.Application.Features.Cities.Commands.Validators
{
    public class UpdateCityValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityValidator()
        {
            RuleFor(x => x.Name)
            .NotNull().WithMessage("City Name is required !!!")
            .MaximumLength(100).WithMessage("City Name should not exeed 100 characters !!!")
            .Matches(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$").WithMessage("City name must have first letter capitalized for each word !!!");
        }
    }
}
