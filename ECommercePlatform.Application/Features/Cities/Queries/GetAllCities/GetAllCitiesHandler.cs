using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetAllCities
{
    public class GetAllCitiesHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetAllCitiesQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult<List<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        {
            var cities = await _unitOfWork.Cities.GetAllAsync();
            var cityDtos = _mapper.Map<List<CityDto>>(cities);
            return AppResult<List<CityDto>>.Success(cityDtos);
        }
    }
}
