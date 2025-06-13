using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
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

        public IQueryable<Country> AsQueryable()
        {
            return _context.Countries;
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
                .AnyAsync(c => c.Name != null &&
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
                !c.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueAsync(string code)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code != null &&
                c.Code.ToLower().Trim() == code.ToLower().Trim() &&
                !c.IsDeleted);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Name != null &&
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
                c.Id != excludeId &&
                !c.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code != null &&
                c.Code.ToLower().Trim() == code.ToLower().Trim() &&
                c.Id != excludeId &&
                !c.IsDeleted);
        }

        public async Task<bool> IsNameAndCodeUniqueAsync(string name, string code)
        {
            return !await _context.Countries
                .AnyAsync(c => (c.Name != null && c.Name.ToLower().Trim() == name.ToLower().Trim()) ||
                               (c.Code != null && c.Code.ToLower().Trim() == code.ToLower().Trim()) &&
                               !c.IsDeleted);
        }

        public Task<Result<(string normalizedName, string normalizedCode)>> EnsureNameAndCodeAreUniqueAsync(string name, string code, Guid? excludeId = null)
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
                    var nameQuery = _context.Countries.Where(c =>
                        c.Name != null &&
                        c.Name.ToLower().Trim() == tuple.normalizedName &&
                        !c.IsDeleted);

                    var codeQuery = _context.Countries.Where(c =>
                        c.Code != null &&
                        c.Code.ToLower().Trim() == tuple.normalizedCode &&
                        !c.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                    {
                        nameQuery = nameQuery.Where(c => c.Id != excludeId.Value);
                        codeQuery = codeQuery.Where(c => c.Id != excludeId.Value);
                    }

                    var nameExists = await nameQuery.AnyAsync();
                    var codeExists = await codeQuery.AnyAsync();

                    if (nameExists && codeExists)
                        return Result.Failure<(string, string)>($"Country with name \"{name}\" and code \"{code}\" already exists.");
                    else if (nameExists)
                        return Result.Failure<(string, string)>($"Country with name \"{name}\" already exists.");
                    else if (codeExists)
                        return Result.Failure<(string, string)>($"Country with code \"{code}\" already exists.");
                    else
                        return Result.Success(tuple);
                });
        }

        // Search function for countries (specifically searching name and code)
        private static IQueryable<Country> ApplyCountrySearch(IQueryable<Country> query, string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(c =>
                (c.Name != null && EF.Functions.Like(c.Name.ToLower(), $"%{searchTerm}%")) ||
                (c.Code != null && EF.Functions.Like(c.Code.ToLower(), $"%{searchTerm}%")));
        }

        // Get paginated countries
        public async Task<PagedResponse<Country>> GetPagedCountriesAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            // Create base filter
            Expression<Func<Country, bool>> baseFilter = activeOnly
                ? c => c.IsActive && !c.IsDeleted
                : c => !c.IsDeleted;

            // Use base paging with country-specific search
            return await GetPagedAsync(
                request,
                baseFilter,
                ApplyCountrySearch,
                cancellationToken);
        }

        // Get paginated country DTOs
        public async Task<PagedResponse<CountryDto>> GetPagedCountryDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedCountriesAsync(request, activeOnly, cancellationToken);

            // Map entities to DTOs
            var dtos = pagedEntities.Items.Select(c => (CountryDto)c).ToList();

            return new PagedResponse<CountryDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}