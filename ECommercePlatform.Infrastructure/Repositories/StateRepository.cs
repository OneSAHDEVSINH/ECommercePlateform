using CSharpFunctionalExtensions;
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
                .AnyAsync(s => s.Name != null &&
                s.Name.ToLower().Trim() == name.ToLower().Trim() &&
                s.CountryId == countryId &&
                !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code != null &&
                s.Code.ToLower().Trim() == code.ToLower().Trim() &&
                s.CountryId == countryId &&
                !s.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Name != null &&
                s.Name.ToLower().Trim() == name.ToLower().Trim() &&
                s.CountryId == countryId &&
                s.Id != excludeId &&
                !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code != null &&
                s.Code.ToLower().Trim() == code.ToLower().Trim() &&
                s.CountryId == countryId &&
                s.Id != excludeId &&
                !s.IsDeleted);
        }

        public Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueInCountryAsync(string name, string code, Guid countryId)
        {
            return Result.Success((name, code))
                // Validate name is not empty
                .Ensure(tuple => !string.IsNullOrEmpty(tuple.name?.Trim()), "Name cannot be null or empty.")
                // Validate code is not empty
                .Ensure(tuple => !string.IsNullOrEmpty(tuple.code?.Trim()), "Code cannot be null or empty.")
                // Normalize the inputs
                .Map(tuple => (
                    normalizedName: tuple.name.Trim().ToLower(),
                    normalizedCode: tuple.code.Trim().ToLower()
                ))
                // Check uniqueness against database
                .Bind(async tuple =>
                {
                    var nameExists = await _context.States
                        .AnyAsync(s => s.Name != null &&
                                  s.Name.ToLower().Trim() == tuple.normalizedName &&
                                  s.CountryId == countryId &&
                                  !s.IsDeleted);

                    var codeExists = await _context.States
                        .AnyAsync(s => s.Code != null &&
                                  s.Code.ToLower().Trim() == tuple.normalizedCode &&
                                  s.CountryId == countryId &&
                                  !s.IsDeleted);

                    if (nameExists && codeExists)
                        return Result.Failure<(string, string)>($"State with name \"{name}\" and code \"{code}\" already exists in this country.");
                    else if (nameExists)
                        return Result.Failure<(string, string)>($"State with name \"{name}\" already exists in this country.");
                    else if (codeExists)
                        return Result.Failure<(string, string)>($"State with code \"{code}\" already exists in this country.");
                    else
                        return Result.Success(tuple);
                });
        }

        // Overload for update operations that excludes a specific ID
        public Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueInCountryAsync(string name, string code, Guid countryId, Guid excludeId)
        {
            return Result.Success((name, code))
                // Validate name is not empty
                .Ensure(tuple => !string.IsNullOrEmpty(tuple.name?.Trim()), "Name cannot be null or empty.")
                // Validate code is not empty
                .Ensure(tuple => !string.IsNullOrEmpty(tuple.code?.Trim()), "Code cannot be null or empty.")
                // Normalize the inputs
                .Map(tuple => (
                    normalizedName: tuple.name.Trim().ToLower(),
                    normalizedCode: tuple.code.Trim().ToLower()
                ))
                // Check uniqueness against database
                .Bind(async tuple =>
                {
                    var nameExists = await _context.States
                        .AnyAsync(s => s.Name != null &&
                                  s.Name.ToLower().Trim() == tuple.normalizedName &&
                                  s.CountryId == countryId &&
                                  s.Id != excludeId &&
                                  !s.IsDeleted);

                    var codeExists = await _context.States
                        .AnyAsync(s => s.Code != null &&
                                  s.Code.ToLower().Trim() == tuple.normalizedCode &&
                                  s.CountryId == countryId &&
                                  s.Id != excludeId &&
                                  !s.IsDeleted);

                    if (nameExists && codeExists)
                        return Result.Failure<(string, string)>($"State with name \"{name}\" and code \"{code}\" already exists in this country.");
                    else if (nameExists)
                        return Result.Failure<(string, string)>($"State with name \"{name}\" already exists in this country.");
                    else if (codeExists)
                        return Result.Failure<(string, string)>($"State with code \"{code}\" already exists in this country.");
                    else
                        return Result.Success(tuple);
                });
        }

    }
}
