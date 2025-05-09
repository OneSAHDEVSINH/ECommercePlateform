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
        public async Task<ActionResult<State>> CreateState([FromBody]State state)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            // Validate if country exists
            var country = await _context.Countries.FindAsync(state.CountryId);
            if (country == null || country.IsDeleted)
            {
                return BadRequest("Invalid Country ID");
            }

            // Check if a state with the same name and code exists
            var existingState = await _context.States
                .FirstOrDefaultAsync(s => s.Name == state.Name && s.Code == state.Code);

            if (existingState != null && existingState.IsDeleted)
            {
                existingState.Name = state.Name;
                existingState.Code = state.Code;
                existingState.ModifiedOn = DateTime.Now;
                existingState.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                existingState.IsActive = true;
                existingState.IsDeleted = false;

                // Use existing state instead of adding a new one
                await _context.SaveChangesAsync();
                Console.WriteLine($"State with ID {existingState.Id} was restored and updated.");
                return CreatedAtAction(nameof(GetState), new { id = existingState.Id }, existingState);

            }
            else if (existingState != null && !existingState.IsDeleted)
            {
                // If state exists and is not deleted, return a conflict response
                ModelState.AddModelError("", $"A state with name '{state.Name}' and code '{state.Code}' already exists");
                return Conflict(ModelState);
            }
            else
            {
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
        }

        // PUT: api/State/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody]State state)
        {
            // Add detailed logging for debugging
            Console.WriteLine($"Received PUT request for state ID: {id}");

            if (state == null)
            {
                return BadRequest("Request body null");
            }

            Console.WriteLine($"Country object: {state.Id}, {state.Name}, {state.Code}, {state.CreatedBy}, {state.ModifiedBy}, {state.ModifiedOn}, {state.IsActive}, {state.IsDeleted}");

            if (id != state.Id)
            {
                Console.WriteLine($"ID mismatch: Path ID={id}, State ID={state.Id}");
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
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

            // Check if there is another country with the same name and code
            var duplicateState = await _context.States
                .FirstOrDefaultAsync(s => s.Id != id && s.Name == state.Name && s.Code == state.Code && !s.IsDeleted);

            if (duplicateState != null)
            {
                ModelState.AddModelError("", $"Another country with name '{country.Name}' and code '{country.Code}' already exists");
                return BadRequest(ModelState);
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
            catch (DbUpdateException ex)
            {
                // Log more details about the exception
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "A database error occurred while updating the country. The country name and code must be unique.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
            state.IsActive = false;
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