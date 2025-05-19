using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommercePlatform.Application.DTOs;

namespace ECommercePlatform.Application.Features.Cities.Queries.Handlers
{
    public class GetCityByIdHandler : IRequestHandler<GetCityByIdQuery, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetCityByIdHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
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
