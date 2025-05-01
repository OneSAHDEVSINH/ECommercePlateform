using ECommercePlateform.Server.Data;
using ECommercePlateform.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StateController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StateController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/State
        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetStates()
        {
            return await _context.States
                .Include(s => s.Country)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        // GET: api/State/5
        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetState(Guid id)
        {
            var state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null || state.IsDeleted)
            {
                return NotFound();
            }

            return state;
        }

        // GET: api/State/ByCountry/5
        [HttpGet("ByCountry/{countryId}")]
        public async Task<ActionResult<IEnumerable<State>>> GetStatesByCountry(Guid countryId)
        {
            var country = await _context.Countries.FindAsync(countryId);
            if (country == null || country.IsDeleted)
            {
                return NotFound("Country not found");
            }

            return await _context.States
                .Where(s => s.CountryId == countryId && !s.IsDeleted)
                .ToListAsync();
        }

        // POST: api/State
        [HttpPost]
        public async Task<ActionResult<State>> CreateState(State state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate if country exists
            var country = await _context.Countries.FindAsync(state.CountryId);
            if (country == null || country.IsDeleted)
            {
                return BadRequest("Invalid Country ID");
            }

            state.Id = Guid.NewGuid();
            state.CreatedOn = DateTime.Now;
            state.ModifiedOn = DateTime.Now;
            state.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
            state.ModifiedBy = state.CreatedBy;
            state.IsActive = true;
            state.IsDeleted = false;

            _context.States.Add(state);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetState), new { id = state.Id }, state);
        }

        // PUT: api/State/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateState(Guid id, State state)
        {
            if (id != state.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingState = await _context.States.FindAsync(id);
            if (existingState == null || existingState.IsDeleted)
            {
                return NotFound();
            }

            // Validate if country exists
            var country = await _context.Countries.FindAsync(state.CountryId);
            if (country == null || country.IsDeleted)
            {
                return BadRequest("Invalid Country ID");
            }

            // Update only allowed fields
            existingState.Name = state.Name;
            existingState.Code = state.Code;
            existingState.CountryId = state.CountryId;
            existingState.IsActive = state.IsActive;
            existingState.ModifiedOn = DateTime.Now;
            existingState.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/State/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteState(Guid id)
        {
            var state = await _context.States.FindAsync(id);
            if (state == null || state.IsDeleted)
            {
                return NotFound();
            }

            // Soft delete
            state.IsDeleted = true;
            state.ModifiedOn = DateTime.Now;
            state.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StateExists(Guid id)
        {
            return _context.States.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
} 