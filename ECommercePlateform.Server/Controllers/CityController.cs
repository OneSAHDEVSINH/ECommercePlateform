using ECommercePlateform.Server.Data;
using ECommercePlateform.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class CityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(Guid id)
        {
            var city = await _context.Cities
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null || city.IsDeleted)
            {
                return NotFound();
            }

            return city;
        }

        // GET: api/City/ByState/5
        [HttpGet("ByState/{stateId}")]
        public async Task<ActionResult<IEnumerable<City>>> GetCitiesByState(Guid stateId)
        {
            var state = await _context.States.FindAsync(stateId);
            if (state == null || state.IsDeleted)
            {
                return NotFound("State not found");
            }

            return await _context.Cities
                .Where(c => c.StateId == stateId && !c.IsDeleted)
                .ToListAsync();
        }

        // POST: api/City
        [HttpPost]
        public async Task<ActionResult<City>> CreateCity(City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate if state exists
            var state = await _context.States.FindAsync(city.StateId);
            if (state == null || state.IsDeleted)
            {
                return BadRequest("Invalid State ID");
            }

            city.Id = Guid.NewGuid();
            city.CreatedOn = DateTime.Now;
            city.ModifiedOn = DateTime.Now;
            city.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
            city.ModifiedBy = city.CreatedBy;
            city.IsActive = true;
            city.IsDeleted = false;

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(Guid id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCity = await _context.Cities.FindAsync(id);
            if (existingCity == null || existingCity.IsDeleted)
            {
                return NotFound();
            }

            // Validate if state exists
            var state = await _context.States.FindAsync(city.StateId);
            if (state == null || state.IsDeleted)
            {
                return BadRequest("Invalid State ID");
            }

            // Update only allowed fields
            existingCity.Name = city.Name;
            existingCity.StateId = city.StateId;
            existingCity.IsActive = city.IsActive;
            existingCity.ModifiedOn = DateTime.Now;
            existingCity.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null || city.IsDeleted)
            {
                return NotFound();
            }

            // Soft delete
            city.IsActive = false;
            city.IsDeleted = true;
            city.ModifiedOn = DateTime.Now;
            city.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return _context.Cities.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
} 