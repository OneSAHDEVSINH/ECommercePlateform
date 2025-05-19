using ECommercePlatform.Application.Features.Countries.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.States.Commands.Validators
{
    public class UpdateStateValidator : AbstractValidator<UpdateStateCommand>
    {
        public UpdateStateValidator()
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
