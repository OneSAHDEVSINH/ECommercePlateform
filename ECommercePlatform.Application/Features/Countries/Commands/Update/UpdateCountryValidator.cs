using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.Countries.Commands.Update
{
    public class UpdateCountryValidator : AbstractValidator<UpdateCountryCommand>
    {
        private static readonly Regex CapitalizedWordsRegex = GeneratedRegex.CapitalizedWords();
        private static readonly Regex UppercaseLettersRegex = GeneratedRegex.UppercaseLetters();

        public UpdateCountryValidator()
        {
            RuleFor(x => x.Name!.Trim())
                .NotNull().WithMessage("Country Name is required !!!")
                .MaximumLength(100).WithMessage("Country Name should not exeed 100 characters !!!")
                .Must(static value => CapitalizedWordsRegex.IsMatch(value))
                .WithMessage("Country name must have first letter capitalized for each word !!!");

            RuleFor(x => x.Code!.Trim())
                .NotNull().WithMessage("Country Code is required !!!")
                .MaximumLength(3).WithMessage("Code should not exceed 3 characters !!!")
                .Must(static value => UppercaseLettersRegex.IsMatch(value))
                .WithMessage("Code must be 1 to 3 uppercase letters !!!");
        }
    }
}
