using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesByCountry
{
    public class GetStatesByCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStatesByCountryQuery, AppResult<List<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<StateDto>>> Handle(GetStatesByCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _unitOfWork.Countries.GetByIdAsync(request.CountryId);
                if (country == null)
                    return AppResult<List<StateDto>>.Failure($"Country with this ID \"{request.CountryId}\" not found.");

                var states = await _unitOfWork.States.GetStatesByCountryIdAsync(request.CountryId);
                var statesDto = states.Select(state => (StateDto)state).ToList();

                return AppResult<List<StateDto>>.Success(statesDto);
            }
            catch (Exception ex)
            {
                return AppResult<List<StateDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
