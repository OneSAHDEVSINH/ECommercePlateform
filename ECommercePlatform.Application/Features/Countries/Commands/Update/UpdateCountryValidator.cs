using FluentValidation;

namespace ECommercePlatform.Application.Features.Countries.Commands.Update
{
    public class UpdateCountryValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryValidator()
        {
            RuleFor(x => x.Name)
            .NotNull().WithMessage("Country Name is required !!!")
            .MaximumLength(100).WithMessage("Country Name should not exeed 100 characters !!!")
            .Matches(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$").WithMessage("Country name must have first letter capitalized for each word !!!");


            RuleFor(x => x.Code)
                .NotNull().WithMessage("Country Code is required !!!")
                .MaximumLength(3).WithMessage("Code should not exceed 3 characters !!!")
                .Matches(@"^[A-Z]{1,3}$").WithMessage("Code must be 1 to 3 uppercase letters !!!");

        }
    }
}
