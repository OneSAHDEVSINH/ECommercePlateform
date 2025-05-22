using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.ICity
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<City> GetCityByIdAsync(Guid id);
        Task<IReadOnlyList<City>> GetActiveCountries();
        Task<IReadOnlyList<City>> GetCitiesByStateIdAsync(Guid stateId);
        Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId);
        Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId, Guid excludeId);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId);
        Task<AppResult<string>> EnsureNameIsUniqueInStateAsync(string name, Guid stateId);
        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);

    }
}
