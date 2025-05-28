using ECommercePlatform.Application.Common.Validation;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public class UpdateStateValidator : AbstractValidator<UpdateStateCommand>
    {
        private static readonly Regex CapitalizedWordsRegex = GeneratedRegex.CapitalizedWords();
        private static readonly Regex UppercaseLettersRegex = GeneratedRegex.UppercaseLetters();

        public UpdateStateValidator()
        {
            RuleFor(x => x.Name!.Trim())
                .NotNull().WithMessage("State Name is required !!!")
                .MaximumLength(100).WithMessage("State Name should not exeed 100 characters !!!")
                .Must(static value => CapitalizedWordsRegex.IsMatch(value))
                .WithMessage("State name must have first letter capitalized for each word !!!");

            RuleFor(x => x.Code!.Trim())
                .NotNull().WithMessage("State Code is required !!!")
                .MaximumLength(3).WithMessage("Code should not exceed 3 characters !!!")
                .Must(static value => UppercaseLettersRegex.IsMatch(value))
                .WithMessage("Code must be 1 to 3 uppercase letters !!!");
        }
    }
}
