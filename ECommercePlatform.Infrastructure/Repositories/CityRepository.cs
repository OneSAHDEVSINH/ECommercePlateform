using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
    {
        public async Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries.AnyAsync(predicate);
        }

        public async Task<IReadOnlyList<City>> GetActiveCitiesAsync()
        {
            return await _context.Cities
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<City?> GetCityWithStateAndCountryAsync(Guid id)
        {
            return await _context.Cities
                .Include(c => c.State!)
                    .ThenInclude(s => s.Country!)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<City>> GetCitiesByStateIdAsync(Guid stateId)
        {
            return await _context.Cities
                .Where(c => c.StateId == stateId && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<City?> GetCityByNameAsync(string name)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted);
        }

        public async Task<City> GetCityByIdAsync(Guid id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted)
                ?? throw new InvalidOperationException($"City with ID {id} not found.");

            return city!;
        }

        public Task<IReadOnlyList<City>> GetActiveCountries()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId)
        {
            return !await _context.Cities
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim() && c.StateId == stateId && !c.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId, Guid excludeId)
        {
            return !await _context.Cities
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim() && c.StateId == stateId && c.Id != excludeId && !c.IsDeleted);
        }

        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
            {
                return AppResult<string>.Failure("Name cannot be null or empty.");
            }

            var exists = await _context.Cities
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"City with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }

        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
            {
                return AppResult<string>.Failure("Name cannot be null or empty.");
            }

            var exists = await _context.Cities
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && c.Id != excludeId && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"City with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }
    }
}

