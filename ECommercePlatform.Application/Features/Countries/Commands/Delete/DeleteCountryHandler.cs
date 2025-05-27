using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Delete
{
    public class DeleteCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCountryCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Result.Success(request.Id)
                    // Find the country
                    .Bind(async id =>
                    {
                        var country = await _unitOfWork.Countries.GetByIdAsync(id);
                        return country == null
                            ? Result.Failure<Domain.Entities.Country>($"Country with ID {id} not found.")
                            : Result.Success(country);
                    })
                    // Check if there are any associated states
                    .Bind(async country =>
                    {
                        var states = await _unitOfWork.States.GetStatesByCountryIdAsync(country.Id);
                        return states != null && states.Any()
                            ? Result.Failure<Domain.Entities.Country>($"Cannot delete country with ID {country.Id} as it has associated states")
                            : Result.Success(country);
                    })
                    // Delete the country
                    .Tap(async country => await _unitOfWork.Countries.DeleteAsync(country))
                    // Map to final result
                    .Map(_ => AppResult.Success());

                return result.IsSuccess
                    ? result.Value
                    : AppResult.Failure(result.Error);

            }
            catch (Exception ex)
            {
                return AppResult.Failure(ex.Message);
            }
        }
    }
}

// Old Method without C Sharp Functional Extension


//var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
//if (country == null)
//    return AppResult.Failure($"Country with ID {request.Id} not found.");

//var states = await _unitOfWork.States.GetStatesByCountryIdAsync(request.Id);
//if (states != null && states.Any())
//    return AppResult.Failure($"Cannot delete country with ID {request.Id} as it has associated states");

//await _unitOfWork.Countries.DeleteAsync(country);
//return AppResult.Success();