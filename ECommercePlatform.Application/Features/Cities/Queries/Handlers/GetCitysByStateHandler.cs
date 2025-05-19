using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Cities.Queries.Handlers
{
    public class GetCitysByStateHandler : IRequestHandler<GetCitiesByStateQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCitysByStateHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
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
                var citiesDto = _mapper.Map<List<CityDto>>(cities);
                return AppResult<List<CityDto>>.Success(citiesDto);
            }
            catch (Exception ex)
            {
                return AppResult<List<CityDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
