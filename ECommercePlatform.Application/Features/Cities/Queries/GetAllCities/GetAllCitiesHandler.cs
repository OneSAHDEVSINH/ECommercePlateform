using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetAllCities
{
    public class GetAllCitiesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCitiesQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cities = await _unitOfWork.Cities.GetAllAsync();
                var cityDtos = cities.Select(city => (CityDto)city).ToList();

                return AppResult<List<CityDto>>.Success(cityDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<CityDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
