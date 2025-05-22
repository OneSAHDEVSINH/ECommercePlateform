using CSharpFunctionalExtensions;
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
        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);
        Task<Result<string>> EnsureNameIsUniqueInStateAsync(string name, Guid stateId);
        // Version with excludeId for updates
        Task<Result<string>> EnsureNameIsUniqueInStateAsync(string name, Guid stateId, Guid excludeId);

    }
}
