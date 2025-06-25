using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public class UpdateCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCountryCommand, AppResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AppResult> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Method Using DTO 
            // Convert command to DTO early
            var updateDto = (UpdateCountryDto)request;

            var result = await Result.Success(updateDto)
                .Bind(async dto =>
                {
                    var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);

                    return country == null
                        ? Result.Failure<(Country country, UpdateCountryDto dto)>($"Country with ID \"{request.Id}\" not found.")
                        : Result.Success((country, dto));
                })
                .Bind(async tuple =>
                {
                    var (country, dto) = tuple;

                    // Validation still needs values, extract from DTO
                    var validationResult = await _unitOfWork.Countries.EnsureNameAndCodeAreUniqueAsync(
                        dto.Name ?? string.Empty,
                        dto.Code ?? string.Empty,
                        request.Id);

                    return validationResult.IsSuccess
                        ? Result.Success((country, dto))
                        : Result.Failure<(Country country, UpdateCountryDto dto)>(validationResult.Error);
                })
                .Tap(async tuple =>
                {
                    var (country, dto) = tuple;

                    // Update entity using values from DTO
                    country.Update(
                        dto.Name ?? string.Empty,
                        dto.Code ?? string.Empty
                    );

                    await _unitOfWork.Countries.UpdateAsync(country);
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