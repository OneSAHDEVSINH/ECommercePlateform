using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetCountryById
{
    public class GetCountryByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCountryByIdQuery, AppResult<CountryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
                if (country == null)                
                    return AppResult<CountryDto>.Failure($"Country with this ID \"{request.Id}\" not found");
                
                var countryDto = (CountryDto)country;

                return AppResult<CountryDto>.Success(countryDto);
            }
            catch (Exception ex)
            {
                return AppResult<CountryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
