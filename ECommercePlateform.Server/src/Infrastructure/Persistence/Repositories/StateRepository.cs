using ECommercePlateform.Server.Core.Domain.Entities;
using ECommercePlateform.Server.Infrastructure.Persistence;
using ECommercePlateform.Server.Infrastructure.Persistence.Repositories;
using ECommercePlateform.Server.src.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Server.src.Infrastructure.Persistence.Repositories
{
    public class StateRepository : GenericRepository<State>, IStateRepository
    {
        public StateRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IReadOnlyList<State>> GetActiveStatesAsync()
        {
            return await _context.States
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<State>> GetStatesByCountryIdAsync(Guid countryId)
        {
            return await _context.States
                .Where(s => s.CountryId == countryId && !s.IsDeleted)
                .Include(s => s.Country)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<State> GetStateWithCitiesAsync(Guid id) 
        {
            var state = await _context.States
                .Include(s => s.Cities!.Where(c => c.IsActive && !c.IsDeleted)) 
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException($"State with ID {id} not found.");

            return state;
        }

        public async Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId)
        {
            return !await _context.States
                .AnyAsync(s => s.Name.ToLower().Trim() == name.ToLower().Trim() && s.CountryId == countryId && !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code!.ToLower().Trim() == code.ToLower().Trim() && s.CountryId == countryId && !s.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Name.ToLower().Trim() == name.ToLower().Trim() && s.CountryId == countryId && s.Id != excludeId && !s.IsDeleted);
        }

        public async Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId, Guid excludeId)
        {
            return !await _context.States
                .AnyAsync(s => s.Code!.ToLower().Trim() == code.ToLower().Trim() && s.CountryId == countryId && s.Id != excludeId && !s.IsDeleted);
        }

    }
}
