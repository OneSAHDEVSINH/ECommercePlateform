using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Cities.Queries.Handlers
{
    public class GetAllCitiesHandler : IRequestHandler<GetAllCitiesQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllCitiesHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<AppResult<List<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        {
            var cities = await _unitOfWork.Cities.GetAllAsync();
            var cityDtos = _mapper.Map<List<CityDto>>(cities);
            return AppResult<List<CityDto>>.Success(cityDtos);
        }
    }
}
