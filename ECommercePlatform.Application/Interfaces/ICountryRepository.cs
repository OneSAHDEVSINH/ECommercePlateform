using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueAsync(string name, string code, Guid? excludeId = null);

        // Paginated countries with optional filtering
        Task<PagedResponse<Country>> GetPagedCountriesAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        // Paginated country DTOs with optional filtering
        Task<PagedResponse<CountryDto>> GetPagedCountryDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}