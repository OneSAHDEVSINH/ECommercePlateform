using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.Handlers
{
    public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, AppResult<List<CountryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetAllCountriesHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<AppResult<List<CountryDto>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Countries.GetAllAsync();
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return AppResult<List<CountryDto>>.Success(countryDtos);
        }
    }
}
