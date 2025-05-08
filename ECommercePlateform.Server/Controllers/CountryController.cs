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
    public class CountryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CountryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            return await _context.Countries
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(Guid id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null || country.IsDeleted)
            {
                return NotFound();
            }

            return country;
        }

        // POST: api/Country
        [HttpPost]
        public async Task<ActionResult<Country>> CreateCountry([FromBody]Country country)
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

            // Check if a country with the same name and code exists
            var existingCountry = await _context.Countries
                .FirstOrDefaultAsync(c => c.Name == country.Name && c.Code == country.Code);

            if (existingCountry != null && existingCountry.IsDeleted)
            {
                // Un-delete the existing country and update its properties
                existingCountry.Name = country.Name;
                existingCountry.Code = country.Code;
                existingCountry.ModifiedOn = DateTime.Now;
                existingCountry.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                existingCountry.IsActive = true;
                existingCountry.IsDeleted = false;

                // Use existing country instead of adding a new one
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCountry), new { id = existingCountry.Id }, existingCountry);
            }
            else if (existingCountry != null && !existingCountry.IsDeleted)
            {
                // If country exists and is not deleted, return a conflict response
                ModelState.AddModelError("", $"A country with name '{country.Name}' and code '{country.Code}' already exists");
                return Conflict(ModelState);
            }
            else
            {
                // Create a new country
                country.Id = Guid.NewGuid();
                country.CreatedOn = DateTime.Now;
                country.ModifiedOn = DateTime.Now;
                country.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                country.ModifiedBy = country.CreatedBy;
                country.IsActive = true;
                country.IsDeleted = false;

                _context.Countries.Add(country);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
            }
        }

        // PUT: api/Country/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(Guid id, [FromBody]Country country)
        {
            // Add detailed logging for debugging
            Console.WriteLine($"Received PUT request for country ID: {id}");

            if (country == null)
            {
                return BadRequest("Request body null");
            }

            Console.WriteLine($"Country object: {country.Id}, {country.Name}, {country.Code}, {country.CreatedBy}, {country.ModifiedBy}, {country.ModifiedOn}, {country.IsActive}, {country.IsDeleted}");

            if (id != country.Id)
            {
                Console.WriteLine($"ID mismatch: Path ID={id}, Country ID={country.Id}");
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

            var existingCountry = await _context.Countries.FindAsync(id);
            if (existingCountry == null || existingCountry.IsDeleted)
            {
                return NotFound();
            }

            // Check if there is another country with the same name and code
            var duplicateCountry = await _context.Countries
                .FirstOrDefaultAsync(c => c.Id != id && c.Name == country.Name && c.Code == country.Code && !c.IsDeleted);

            if (duplicateCountry != null)
            {
                ModelState.AddModelError("", $"Another country with name '{country.Name}' and code '{country.Code}' already exists");
                return BadRequest(ModelState);
            }

            // Update only allowed fields
            existingCountry.Name = country.Name;
            existingCountry.Code = country.Code;
            existingCountry.ModifiedOn = DateTime.Now;
            existingCountry.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
            existingCountry.IsActive = country.IsActive;
            existingCountry.IsDeleted = country.IsDeleted;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!CountryExists(id))
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

        // DELETE: api/Country/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null || country.IsDeleted)
            {
                return NotFound();
            }

            // Soft delete
            country.IsActive = false;
            country.IsDeleted = true;
            country.ModifiedOn = DateTime.Now;
            country.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(Guid id)
        {
            return _context.Countries.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
} 