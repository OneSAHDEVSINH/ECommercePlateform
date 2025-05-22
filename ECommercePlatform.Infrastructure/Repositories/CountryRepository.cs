using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.ICountry;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class CountryRepository(AppDbContext context) : GenericRepository<Country>(context), ICountryRepository
    {
        public async Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries.AnyAsync(predicate);
        }
        public async Task<IReadOnlyList<Country>> GetActiveCountriesAsync()
        {
            return await _context.Countries
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Country> GetCountryWithStatesAsync(Guid id)
        {
            var country = await _context.Countries
                .Include(c => c.States!.Where(s => s.IsActive && !s.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException($"Country with ID {id} not found.");

            return country!;
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim() && !c.IsDeleted);
        }



        public async Task<bool> IsCodeUniqueAsync(string code)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == code.ToLower().Trim() && !c.IsDeleted);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim() && c.Id != excludeId && !c.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == code.ToLower().Trim() && c.Id != excludeId && !c.IsDeleted);
        }

        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
            {
                return AppResult<string>.Failure("Name cannot be null or empty.");
            }

            var exists = await _context.Countries
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"Country with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }

        public async Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code)
        {
            var normalizedCode = code?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedCode))
            {
                return AppResult<string>.Failure("Code cannot be null or empty.");
            }

            var exists = await _context.Countries
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == normalizedCode && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"Country with this code \"{code}\" already exists.")
                : AppResult<string>.Success(normalizedCode);
        }

        public Task<Result<string>> EnsureCodeAIsUniqueAsync(string code)
        {
            return Result.Success(code)
                .Ensure(c => !string.IsNullOrEmpty(c?.Trim()), "Code cannot be null or empty.")
                .Map(c => c.Trim().ToLower())
                .Bind(async normalizedCode =>
                {
                    var exists = await _context.Countries
                        .AnyAsync(c => c.Code != null &&
                                       c.Code.ToLower().Trim() == normalizedCode &&
                                       !c.IsDeleted);

                    return exists
                        ? Result.Failure<string>($"Country with this code \"{code}\" already exists.")
                        : Result.Success(normalizedCode);
                });
        }

        public Task<Result<string>> EnsureNameAIsUniqueAsync(string name)
        {
            return Result.Success(name)
                .Ensure(c => !string.IsNullOrEmpty(c?.Trim()), "Name cannot be null or empty.")
                .Map(c => c.Trim().ToLower())
                .Bind(async normalizedName =>
                {
                    var exists = await _context.Countries
                        .AnyAsync(c => c.Name != null &&
                                       c.Name.ToLower().Trim() == normalizedName &&
                                       !c.IsDeleted);

                    return exists
                        ? Result.Failure<string>($"Country with this code \"{name}\" already exists.")
                        : Result.Success(normalizedName);
                });
        }

        public async Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId)
        {
            var normalizedName = name?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedName))
                return AppResult<string>.Failure("Name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(name))
                return AppResult<string>.Failure("Country name cannot be null or empty.");


            var exists = await _context.Countries
                .AnyAsync(c => c.Name != null && c.Name.ToLower().Trim() == normalizedName && c.Id != excludeId && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"Country with this name \"{name}\" already exists.")
                : AppResult<string>.Success(normalizedName);
        }

        public async Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code, Guid excludeId)
        {
            var normalizedCode = code?.Trim().ToLower();
            if (string.IsNullOrEmpty(normalizedCode))
                return AppResult<string>.Failure("Code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(code))
                return AppResult<string>.Failure("Country code cannot be null or empty.");

            var exists = await _context.Countries
                .AnyAsync(c => c.Code != null && c.Code.ToLower().Trim() == normalizedCode && c.Id != excludeId && !c.IsDeleted);

            return exists ? AppResult<string>.Failure($"Country with this code \"{code}\" already exists.")
                : AppResult<string>.Success(normalizedCode);
        }
        public Task<Result<string>> EnsureCodeAIsUniqueAsync(string code, Guid excludeId)
        {
            return Task.FromResult(Result.Success(code))
                .Ensure(c => !string.IsNullOrEmpty(c?.Trim()), "Code cannot be null or empty.")
                .Ensure(c => !string.IsNullOrWhiteSpace(c), "Country code cannot be null or empty.")
                .Map(c => c.Trim().ToLower())
                .Bind(async normalizedCode =>
                {
                    var exists = await _context.Countries
                        .AnyAsync(c => c.Code != null &&
                                       c.Code.ToLower().Trim() == normalizedCode &&
                                       c.Id != excludeId &&
                                       !c.IsDeleted);

                    return exists
                        ? Result.Failure<string>($"Country with this code \"{code}\" already exists.")
                        : Result.Success(normalizedCode);
                });
        }
        private Task<Result<string>> ValidateCode(string code, Guid countryId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Task.FromResult(Result.Failure<string>("Country code cannot be null or empty."));

            return IsCodeUniqueAsync(code, countryId)
            .ContinueWith(task =>
            task.Result
            ? Result.Success(code)
            : Result.Failure<string>($"Country with this code \"{code}\" already exists."));
        }

        private Task<Result<string>> ValidateName(string name, Guid countryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Task.FromResult(Result.Failure<string>("Country name cannot be null or empty."));

            return IsNameUniqueAsync(name, countryId)
            .ContinueWith(task =>
            task.Result
            ? Result.Success(name)
            : Result.Failure<string>($"Country with this name \"{name}\" already exists."));
        }

        public async Task<bool> IsNameAndCodeUniqueAsync(string name, string code)
        {
            return !await _context.Countries
                .AnyAsync(c => (c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim()) ||
                               (c.Code != null && c.Code.ToLower().Trim() == code.ToLower().Trim()) &&
                               !c.IsDeleted);
        }
    }
}