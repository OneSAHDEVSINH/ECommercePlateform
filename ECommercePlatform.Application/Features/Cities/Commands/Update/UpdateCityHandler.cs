using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public class UpdateCityHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCityCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Method Using DTO 
                // Convert command to DTO early
                var updateDto = (UpdateCityDto)request;

                var result = await Result.Success(updateDto)
                    .Bind(async dto =>
                    {
                        var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
                        return city == null
                            ? Result.Failure<(Domain.Entities.City city, UpdateCityDto dto)>($"City with ID \"{request.Id}\" not found.")
                            : Result.Success((city, dto));
                    })
                    .Bind(async tuple =>
                    {
                        var (city, dto) = tuple;

                        // Validation still needs values, extract from DTO
                        var validationResult = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(
                            dto.Name ?? string.Empty, 
                            request.StateId,
                            request.Id);

                        return validationResult.IsSuccess
                            ? Result.Success((city, dto))
                            : Result.Failure<(Domain.Entities.City city, UpdateCityDto dto)>(validationResult.Error);
                    })
                    .Tap(async tuple =>
                    {
                        var (city, dto) = tuple;

                        // Update entity using values from DTO
                        city.Update(
                            dto.Name ?? string.Empty,
                            dto.StateId
                        );

                        await _unitOfWork.Cities.UpdateAsync(city);
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

//if (string.IsNullOrWhiteSpace(request.Name))
//    return AppResult.Failure("City name cannot be null or empty.");

//var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
//if (city == null)
//    return AppResult.Failure($"City with this ID \"{request.Id}\" not found.");

//var isNameUniqueInState = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(request.Name, request.StateId, request.Id);
//if (isNameUniqueInState.IsFailure)
//    return AppResult.Failure(isNameUniqueInState.Error);

//var updatedCity = (UpdateCityDto)request;
//city.Update(request.Name, request.StateId);

//await _unitOfWork.Cities.UpdateAsync(city);
//return AppResult.Success();