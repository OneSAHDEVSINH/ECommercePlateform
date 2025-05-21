using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<GetCityByIdQuery, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
            if (city == null)
            {
                return AppResult<CityDto>.Failure($"City with this ID \"{request.Id}\" not found.");
            }
            var cityDto = _mapper.Map<CityDto>(city);
            return AppResult<CityDto>.Success(cityDto);


        }
    }
}
