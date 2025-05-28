using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public class UpdateCityValidator : AbstractValidator<UpdateCityCommand>
    {
        private static readonly Regex CapitalizedWordsRegex = GeneratedRegex.CapitalizedWords();

        public UpdateCityValidator()
        {
            RuleFor(x => x.Name.Trim())
            .NotNull().WithMessage("City Name is required !!!")
            .MaximumLength(100).WithMessage("City Name should not exeed 100 characters !!!")
            .Must(static value => CapitalizedWordsRegex.IsMatch(value))
                .WithMessage("City name must have first letter capitalized for each word !!!");
        }
    }
}
