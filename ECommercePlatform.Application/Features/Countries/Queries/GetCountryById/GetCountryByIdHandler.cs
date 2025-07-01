using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetCountryById
{
    public class GetCountryByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCountryByIdQuery, AppResult<CountryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req => {
                        var country = await _unitOfWork.Countries.GetByIdAsync(req.Id);
                        return country == null
                            ? Result.Failure<Country>($"Country with this ID \"{req.Id}\" not found")
                            : Result.Success(country);
                    })
                    .Map(country => (CountryDto)country)
                    .Map(countryDto => AppResult<CountryDto>.Success(countryDto))
                    .Match(
                        success => success,
                        failure => AppResult<CountryDto>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<CountryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
