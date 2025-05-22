using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetCitiesByState
{
    public class GetCitysByStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCitiesByStateQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<CityDto>>> Handle(GetCitiesByStateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var state = await _unitOfWork.States.GetByIdAsync(request.StateId);
                if (state == null)
                {
                    return AppResult<List<CityDto>>.Failure($"Cities with this ID \"{request.StateId}\" not found.");
                }
                var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(request.StateId);
                //var citiesDto = _mapper.Map<List<CityDto>>(cities);
                //var cityDtos = _mapper.Map<List<CityDto>>(cities);
                var citiesDto = cities.Select(city => (CityDto)city).ToList();
                return AppResult<List<CityDto>>.Success(citiesDto);
            }
            catch (Exception ex)
            {
                return AppResult<List<CityDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
