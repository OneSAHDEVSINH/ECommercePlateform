using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IState
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
        Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueInCountryAsync(string name, string code, Guid countryId);
        Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueInCountryAsync(string name, string code, Guid countryId, Guid excludeId);

        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);

    }
}
