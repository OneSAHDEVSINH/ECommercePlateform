using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.Pagination;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Application.Services;
using ECommercePlatform.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries
{
    public class GetPagedCountriesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedCountriesQuery, AppResult<PagedResponse<CountryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<CountryDto>>> Handle(GetPagedCountriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.Countries.GetPagedCountryDtosAsync(
                    request,
                    activeOnly: true,
                    cancellationToken);

                return AppResult<PagedResponse<CountryDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<CountryDto>>.Failure($"An error occurred while retrieving countries: {ex.Message}");
            }
        }
    }
}