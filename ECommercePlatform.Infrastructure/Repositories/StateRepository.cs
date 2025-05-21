using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class StateRepository(AppDbContext context) : GenericRepository<State>(context), IStateRepository
    {
        public async Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries.AnyAsync(predicate);
        }
        public async Task<IReadOnlyList<State>> GetActiveStatesAsync()
        {
            return await _context.States
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<State>> GetStatesByCountryIdAsync(Guid countryId)
        {
            return await _context.States
                .Where(s => s.CountryId == countryId && !s.IsDeleted)
                .Include(s => s.Country)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<State> GetStateWithCitiesAsync(Guid id)
        {
            var state = await _context.States
                .Include(s => s.Cities!.Where(c => c.IsActive && !c.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException($"State with ID {id} not found.");

            return state!;
        }

        public async Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId)
        {
            return !await _context.States
                .AnyAsync(s => s.Name != null && s.Name.ToLower().Trim() == name.ToLower().Trim() && s.CountryId == countryId && !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code != null && s.Code.ToLower().Trim() == code.ToLower().Trim() && s.CountryId == countryId && !s.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Name != null && s.Name.ToLower().Trim() == name.ToLower().Trim() && s.CountryId == countryId && s.Id != excludeId && !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code != null && s.Code.ToLower().Trim() == code.ToLower().Trim() && s.CountryId == countryId && s.Id != excludeId && !s.IsDeleted);
        }
        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
            {
                return AppResult<string>.Failure("Name cannot be null or empty.");
            }

            var exists = await _context.States
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"State with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }

        public async Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code)
        {
            var normalizedCode = code?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedCode))
            {
                return AppResult<string>.Failure("Code cannot be null or empty.");
            }

            var exists = await _context.States
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == normalizedCode && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"State with this code \"{code}\" already exists.")
                : AppResult<string>.Success(normalizedCode);
        }

        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
            {
                return AppResult<string>.Failure("Name cannot be null or empty.");
            }

            var exists = await _context.States
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && c.Id != excludeId && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"State with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }

        public async Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code, Guid excludeId)
        {
            var normalizedCode = code?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedCode))
            {
                return AppResult<string>.Failure("Code cannot be null or empty.");
            }

            var exists = await _context.States
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == normalizedCode && c.Id != excludeId && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"State with this code \"{code}\" already exists.")
                : AppResult<string>.Success(normalizedCode);
        }
    }
}
