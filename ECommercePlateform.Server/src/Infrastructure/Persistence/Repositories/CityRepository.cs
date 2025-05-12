using ECommercePlateform.Server.Core.Domain.Entities;
using ECommercePlateform.Server.Infrastructure.Persistence;
using ECommercePlateform.Server.Infrastructure.Persistence.Repositories;
using ECommercePlateform.Server.src.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Server.src.Infrastructure.Persistence.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<City>> GetActiveCitiesAsync()
        {
            return await _context.Cities
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City> GetCityWithStateAndCountryAsync(Guid id)
        {
            return await _context.Cities
                .Include(c => c.State)
                    .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<City>> GetCitiesByStateIdAsync(Guid stateId)
        {
            return await _context.Cities
                .Where(c => c.StateId == stateId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<City> GetCityByNameAsync(string name)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted);
        }

        public Task<City> GetCityByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<City>> GetActiveCountries()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId)
        {
            return !await _context.Cities
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.StateId == stateId && !c.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId, Guid excludeId)
        {
            return !await _context.Cities
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.StateId == stateId && c.Id != excludeId && !c.IsDeleted);
        }

    }
}
