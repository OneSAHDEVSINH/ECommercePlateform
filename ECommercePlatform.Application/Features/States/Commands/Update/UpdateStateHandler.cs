using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public class UpdateStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateStateCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Method Using DTO 
                // Convert command to DTO early
                var updateDto = (UpdateStateDto)request;

                var result = await Result.Success(updateDto)
                    .Bind(async dto =>
                    {
                        var state = await _unitOfWork.States.GetByIdAsync(request.Id);

                        return state == null
                            ? Result.Failure<(Domain.Entities.State state, UpdateStateDto dto)>($"State with ID \"{request.Id}\" not found.")
                            : Result.Success((state, dto));
                    })
                    .Bind(async tuple =>
                    {
                        var (state, dto) = tuple;

                        // Validation still needs values, extract from DTO
                        var validationResult = await _unitOfWork.States.EnsureNameAndCodeAreUniqueInCountryAsync(
                            dto.Name ?? string.Empty,
                            dto.Code ?? string.Empty,
                            request.CountryId,
                            request.Id);

                        return validationResult.IsSuccess
                            ? Result.Success((state, dto))
                            : Result.Failure<(Domain.Entities.State state, UpdateStateDto dto)>(validationResult.Error);
                    })
                    .Tap(async tuple =>
                    {
                        var (state, dto) = tuple;

                        // Update entity using values from DTO
                        state.Update(
                            dto.Name ?? string.Empty,
                            dto.Code ?? string.Empty,
                            dto.CountryId
                        );

                        await _unitOfWork.States.UpdateAsync(state);
                    })
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

//old method without CSharp Functional Extension

//if (string.IsNullOrWhiteSpace(request.Code))
//    return AppResult.Failure("State code cannot be null or empty.");

//if (string.IsNullOrWhiteSpace(request.Name))
//    return AppResult.Failure("State name cannot be null or empty.");

//var state = await _unitOfWork.States.GetByIdAsync(request.Id);
//if (state == null)
//    return AppResult.Failure($"State with this ID \"{request.Id}\" not found.");

//var validationResult = await _unitOfWork.States
//    .EnsureNameAndCodeAreUniqueInCountryAsync(request.Name, request.Code, request.CountryId, request.Id);

//if (validationResult.IsFailure)
//    return AppResult.Failure(validationResult.Error);

//var updatedState = (UpdateStateDto)request;
//state.Update(request.Name, request.Code, request.CountryId);

//await _unitOfWork.States.UpdateAsync(state);
//return AppResult.Success();