using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Create;

public class CreateCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCountryCommand, AppResult<CountryDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AppResult<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //var isCodeUnique = await _unitOfWork.Countries.IsCodeUniqueAsync(request.Code);
            //if (!isCodeUnique)
            //{
            //    return AppResult<CountryDto>.Failure($"Country with this code \"{request.Code}\" already exists.");
            //}

            //var isNameUnique = await _unitOfWork.Countries.IsNameUniqueAsync(request.Name);
            //if (!isNameUnique)
            //{
            //    return AppResult<CountryDto>.Failure($"Country with this name \"{request.Name}\" already exists.");
            //}
            //var validationResult = await _unitOfWork.Countries.EnsureNameAndCodeAreUniqueAsync(request.Name, request.Code);

            //if (validationResult.IsFailure)
            //    return AppResult<CountryDto>.Failure(validationResult.Error);

            //var (normalizedName, normalizedCode) = validationResult.Value;

            //var country = Country.Create(normalizedName, normalizedCode);

            //var country = Country.Create(request.Name, request.Code);
            //country.IsActive = true;

            //await _unitOfWork.Countries.AddAsync(country);
            ////await _unitOfWork.CompleteAsync();
            //var countryDto = (CountryDto)country;

            ////var countryDto = _mapper.Map<CountryDto>(country);
            //return AppResult<CountryDto>.Success(countryDto);



            //    return await _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code)
            //.BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name))
            //.BindAsync(_ =>
            //{
            //    var country = Country.Create(request.Name, request.Code);
            //    country.IsActive = true;
            //    return _unitOfWork.Countries.AddAsync(country)
            //        .ContinueWith(_ => AppResult<CountryDto>.Success((CountryDto)country));
            //});

            var result = await _unitOfWork.Countries.EnsureNameAndCodeAreUniqueAsync(request.Name, request.Code)
            .Map(tuple =>
            {
                var country = Country.Create(request.Name, request.Code);
                country.IsActive = true;
                return country;
            })
            .Tap(country => _unitOfWork.Countries.AddAsync(country))
            .Map(country => AppResult<CountryDto>.Success((CountryDto)country));

            return result.IsSuccess
                ? result.Value
                : AppResult<CountryDto>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return AppResult<CountryDto>.Failure($"An error occurred: {ex.Message}");
        }
    }
}