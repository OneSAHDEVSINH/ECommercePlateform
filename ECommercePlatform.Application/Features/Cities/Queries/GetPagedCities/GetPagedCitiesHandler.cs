using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetPagedCities
{
    public class GetPagedCitiesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedCitiesQuery, AppResult<PagedResponse<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<CityDto>>> Handle(GetPagedCitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.Cities.GetPagedCityDtosAsync(
                    request,
                    request.StateId,
                    request.CountryId,
                    request.ActiveOnly,
                    cancellationToken);

                return AppResult<PagedResponse<CityDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<CityDto>>.Failure(
                    $"An error occurred while retrieving cities: {ex.Message}");
            }
        }
    }
}