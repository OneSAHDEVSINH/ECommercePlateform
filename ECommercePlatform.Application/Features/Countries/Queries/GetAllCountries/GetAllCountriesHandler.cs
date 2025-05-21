using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<GetAllCountriesQuery, AppResult<List<CountryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult<List<CountryDto>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Countries.GetAllAsync();
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return AppResult<List<CountryDto>>.Success(countryDtos);
        }
    }
}
