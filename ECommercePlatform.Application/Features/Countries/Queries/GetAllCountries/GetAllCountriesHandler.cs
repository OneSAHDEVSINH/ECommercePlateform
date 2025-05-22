using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCountriesQuery, AppResult<List<CountryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<CountryDto>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAllAsync();
                var countryDtos = countries.Select(country => (CountryDto)country).ToList();

                return AppResult<List<CountryDto>>.Success(countryDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<CountryDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
