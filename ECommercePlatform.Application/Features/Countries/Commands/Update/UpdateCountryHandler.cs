using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
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
                        ? Result.Failure<(Domain.Entities.Country country, UpdateCountryDto dto)>($"Country with ID \"{request.Id}\" not found.")
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
                        : Result.Failure<(Domain.Entities.Country country, UpdateCountryDto dto)>(validationResult.Error);
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

//old method without CSharp Functional Extension

//            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
//            if (country == null)
//                return AppResult.Failure($"Country with this ID \"{request.Id}\" not found.");

//            if (string.IsNullOrWhiteSpace(request.Code))
//                return AppResult.Failure("Country code cannot be null or empty.");

//            if (string.IsNullOrWhiteSpace(request.Name))
//                return AppResult.Failure("Country name cannot be null or empty.");

//            var validationResult = await _unitOfWork.Countries
//                .EnsureNameAndCodeAreUniqueAsync(request.Name, request.Code, request.Id);

//            if (validationResult.IsFailure)
//                return AppResult.Failure(validationResult.Error);

//            var updatedCountry = (UpdateCountryDto)request;
//            country.Update(request.Name, request.Code);

//            await _unitOfWork.Countries.UpdateAsync(country);
//            return AppResult.Success();

//Method using Direct Entity

//var result = await Result.Success(request)
//    .Bind(async r =>
//    {
//        var country = await _unitOfWork.Countries.GetByIdAsync(r.Id);
//        return country == null
//            ? Result.Failure<(Domain.Entities.Country country, string name, string code)>($"Country with ID \"{r.Id}\" not found.")
//            : Result.Success((country, r.Name!, r.Code!));
//    })
//    .Bind(async tuple =>
//    {
//        var (country, name, code) = tuple;
//        var validationResult = await _unitOfWork.Countries.EnsureNameAndCodeAreUniqueAsync(
//            name, code, country.Id);

//        return validationResult.IsSuccess
//            ? Result.Success(country)
//            : Result.Failure<Domain.Entities.Country>(validationResult.Error);
//    })
//    .Tap(async country =>
//    {
//        country.Update(request.Name!, request.Code!);
//        await _unitOfWork.Countries.UpdateAsync(country);
//    })
//    .Map(_ => AppResult.Success());

//return result.IsSuccess
//    ? result.Value
//    : AppResult.Failure(result.Error);