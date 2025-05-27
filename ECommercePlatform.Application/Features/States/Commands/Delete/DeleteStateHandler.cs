using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Delete
{
    public class DeleteStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteStateCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Result.Success(request.Id)
                    // Find the country
                    .Bind(async id =>
                    {
                        var state = await _unitOfWork.States.GetByIdAsync(id);
                        return state == null
                            ? Result.Failure<Domain.Entities.State>($"State with ID {id} not found.")
                            : Result.Success(state);
                    })
                    // Check if there are any associated states
                    .Bind(async state =>
                    {
                        var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(state.Id);
                        return cities != null && cities.Any()
                            ? Result.Failure<Domain.Entities.State>($"Cannot delete state with ID {state.Id} as it has associated states")
                            : Result.Success(state);
                    })
                    // Delete the country
                    .Tap(async state => await _unitOfWork.States.DeleteAsync(state))
                    // Map to final result
                    .Map(_ => AppResult.Success());

                return result.IsSuccess
                    ? result.Value
                    : AppResult.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}

//Old Method without using C Sharp Functional Extension

//var state = await _unitOfWork.States.GetByIdAsync(request.Id);
//if (state == null)
//    return AppResult.Failure($"State with ID {request.Id} not found");

//var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(request.Id);
//if (cities != null && cities.Any())
//    return AppResult.Failure($"Cannot delete state with ID {request.Id} as it has associated countries");

//await _unitOfWork.States.DeleteAsync(state);
//return AppResult.Success();
