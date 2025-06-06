using ECommercePlatform.Server.Core.Application.Interfaces;
using ECommercePlatform.Server.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePlatform.Server.Infrastructure.Persistence.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Country>> GetActiveCountriesAsync()
        {
            return await _context.Countries
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Country> GetCountryWithStatesAsync(Guid id)
        {
            var country = await _context.Countries
                .Include(c => c.States!.Where(s => s.IsActive && !s.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException($"Country with ID {id} not found.");

            return country;
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && !c.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueAsync(string code)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code!.ToLower().Trim() == code.ToLower().Trim() && !c.IsDeleted);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && c.Id != excludeId && !c.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, Guid excludeId)
        {
            return !await _context.Countries
                .AnyAsync(c => c.Code!.ToLower().Trim() == code.ToLower().Trim() && c.Id != excludeId && !c.IsDeleted);
        }
    }
} 