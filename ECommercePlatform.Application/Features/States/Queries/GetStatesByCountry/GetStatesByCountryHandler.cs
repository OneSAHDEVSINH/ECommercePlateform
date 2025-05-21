using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesByCountry
{
    public class GetStatesByCountryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetStatesByCountryQuery, AppResult<List<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<AppResult<List<StateDto>>> Handle(GetStatesByCountryQuery request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.CountryId);

            if (country == null)
            {
                return AppResult<List<StateDto>>.Failure($"Country with this ID \"{request.CountryId}\" not found.");
            }

            var states = await _unitOfWork.States.GetStatesByCountryIdAsync(request.CountryId);
            var statesDto = _mapper.Map<List<StateDto>>(states);
            return AppResult<List<StateDto>>.Success(statesDto);
        }
    }
}
