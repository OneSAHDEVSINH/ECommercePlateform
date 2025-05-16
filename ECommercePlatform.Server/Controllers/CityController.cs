//using ECommercePlatform.Server.Data;
//using ECommercePlatform.Server.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace ECommercePlatform.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize(Roles = "Admin")]
//    public class CityController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public CityController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/City
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<City>>> GetCities()
//        {
//            return await _context.Cities
//                .Include(c => c.State)
//                .ThenInclude(s => s.Country)
//                .Where(c => !c.IsDeleted)
//                .ToListAsync();
//        }

//        // GET: api/City/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<City>> GetCity(Guid id)
//        {
//            var city = await _context.Cities
//                .Include(c => c.State)
//                .ThenInclude(s => s.Country)
//                .FirstOrDefaultAsync(c => c.Id == id);

//            if (city == null || city.IsDeleted)
//            {
//                return NotFound();
//            }

//            return city;
//        }

//        // GET: api/City/ByState/5
//        [HttpGet("ByState/{stateId}")]
//        public async Task<ActionResult<IEnumerable<City>>> GetCitiesByState(Guid stateId)
//        {
//            var state = await _context.States.FindAsync(stateId);
//            if (state == null || state.IsDeleted)
//            {
//                return NotFound("State not found");
//            }

//            return await _context.Cities
//                .Where(c => c.StateId == stateId && !c.IsDeleted)
//                .ToListAsync();
//        }

//        // POST: api/City
//        [HttpPost]
//        public async Task<ActionResult<City>> CreateCity([FromBody]City city)
//        {
//            if (!ModelState.IsValid)
//            {
//                foreach (var modelState in ModelState.Values)
//                {
//                    foreach (var error in modelState.Errors)
//                    {
//                        Console.WriteLine($"Model error: {error.ErrorMessage}");
//                    }
//                }
//                return BadRequest(ModelState);
//            }

//            // Validate if state exists
//            var state = await _context.States.FindAsync(city.StateId);
//            if (state == null || state.IsDeleted)
//            {
//                return BadRequest("Invalid State ID");
//            }

//            // Check if a state with the same name and code exists
//            var existingCity = await _context.Cities
//                .FirstOrDefaultAsync(c => c.Name == city.Name);

//            if (existingCity != null && existingCity.IsDeleted)
//            {
//                existingCity.Name = city.Name;
//                existingCity.ModifiedOn = DateTime.Now;
//                existingCity.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
//                existingCity.IsActive = true;
//                existingCity.IsDeleted = false;

//                // Use existing state instead of adding a new one
//                await _context.SaveChangesAsync();
//                Console.WriteLine($"City with ID {existingCity.Id} was restored and updated.");
//                return CreatedAtAction(nameof(GetCity), new { id = existingCity.Id }, existingCity);

//            }
//            else if (existingCity != null && !existingCity.IsDeleted)
//            {
//                // If state exists and is not deleted, return a conflict response
//                ModelState.AddModelError("", $"A state with name '{state.Name}' and code '{state.Code}' already exists");
//                return Conflict(ModelState);
//            }
//            else
//            {
//                city.Id = Guid.NewGuid();
//                city.CreatedOn = DateTime.Now;
//                city.ModifiedOn = DateTime.Now;
//                city.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
//                city.ModifiedBy = city.CreatedBy;
//                city.IsActive = true;
//                city.IsDeleted = false;

//                _context.Cities.Add(city);
//                await _context.SaveChangesAsync();

//                return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
//            }
//        }

//        // PUT: api/City/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateCity(Guid id, [FromBody]City city)
//        {
//            // Add detailed logging for debugging
//            Console.WriteLine($"Received PUT request for state ID: {id}");

//            if (city == null)
//            {
//                return BadRequest("Request body null");
//            }

//            Console.WriteLine($"Country object: {city.Id}, {city.Name}, {city.CreatedBy}, {city.ModifiedBy}, {city.ModifiedOn}, {city.IsActive}, {city.IsDeleted}");

//            if (id != city.Id)
//            {
//                Console.WriteLine($"ID mismatch: Path ID={id}, City ID={city.Id}");
//                return BadRequest("ID mismatch");
//            }

//            if (!ModelState.IsValid)
//            {
//                foreach (var modelState in ModelState.Values)
//                {
//                    foreach (var error in modelState.Errors)
//                    {
//                        Console.WriteLine($"Model error: {error.ErrorMessage}");
//                    }
//                }
//                return BadRequest(ModelState);
//            }

//            var existingCity = await _context.Cities.FindAsync(id);
//            if (existingCity == null || existingCity.IsDeleted)
//            {
//                return NotFound();
//            }

//            // Validate if state exists
//            var state = await _context.States.FindAsync(city.StateId);
//            if (state == null || state.IsDeleted)
//            {
//                return BadRequest("Invalid State ID");
//            }

//            // Update only allowed fields
//            existingCity.Name = city.Name;
//            existingCity.StateId = city.StateId;
//            existingCity.IsActive = city.IsActive;
//            existingCity.ModifiedOn = DateTime.Now;
//            existingCity.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CityExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            catch (DbUpdateException ex)
//            {
//                // Log more details about the exception
//                Console.WriteLine($"DbUpdateException: {ex.Message}");
//                if (ex.InnerException != null)
//                {
//                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
//                }
//                return StatusCode(500, "A database error occurred while updating the country. The country name and code must be unique.");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error updating student: {ex.Message}");
//                Console.WriteLine($"Stack trace: {ex.StackTrace}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }

//            return NoContent();
//        }

//        // DELETE: api/City/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCity(Guid id)
//        {
//            var city = await _context.Cities.FindAsync(id);
//            if (city == null || city.IsDeleted)
//            {
//                return NotFound();
//            }

//            // Soft delete
//            city.IsActive = false;
//            city.IsDeleted = true;
//            city.ModifiedOn = DateTime.Now;
//            city.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool CityExists(Guid id)
//        {
//            return _context.Cities.Any(e => e.Id == id && !e.IsDeleted);
//        }
//    }
//} 