using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries
{
    public class GetPagedCountriesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedCountriesQuery, AppResult<PagedResponse<CountryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<CountryDto>>> Handle(GetPagedCountriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.Countries.GetPagedCountryDtosAsync(
                    request,
                    request.ActiveOnly,
                    cancellationToken);

                return AppResult<PagedResponse<CountryDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<CountryDto>>.Failure($"An error occurred while retrieving countries: {ex.Message}");
            }
        }
    }
}