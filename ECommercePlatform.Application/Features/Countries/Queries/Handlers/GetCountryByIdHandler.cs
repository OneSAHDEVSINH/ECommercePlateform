using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.Handlers
{
    public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, AppResult<CountryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetCountryByIdHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<AppResult<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
            if (country == null)
            {
                return AppResult<CountryDto>.Failure($"Country with this ID \"{request.Id}\" not found");
            }
            var countryDto = _mapper.Map<CountryDto>(country);
            return AppResult<CountryDto>.Success(countryDto);
        }
    }
}
