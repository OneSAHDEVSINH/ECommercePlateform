using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IStateRepository : IGenericRepository<State>
    {
        Task<State> GetStateWithCitiesAsync(Guid id);
        Task<IReadOnlyList<State>> GetActiveStatesAsync();
        Task<IReadOnlyList<State>> GetStatesByCountryIdAsync(Guid countryId);
        Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId);
        Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId);
        Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId, Guid excludeId);
        Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId, Guid excludeId);
        Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueInCountryAsync(string name, string code, Guid countryId, Guid? excludeId = null);
        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);

        // Add pagination methods
        Task<PagedResponse<State>> GetPagedStatesAsync(
            PagedRequest request,
            Guid? countryId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<StateDto>> GetPagedStateDtosAsync(
            PagedRequest request,
            Guid? countryId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

    }
}
