using ECommercePlateform.Server.Core.Application.DTOs;
using ECommercePlateform.Server.src.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlateform.Server.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStates()
        {
            var states = await _stateService.GetAllStatesAsync();
            return Ok(states);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStateById(Guid id)
        {
            try
            {
                var state = await _stateService.GetStateByIdAsync(id);
                return Ok(state);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/cities")]
        public async Task<IActionResult> GetStateWithCities(Guid id)
        {
            try
            {
                var state = await _stateService.GetStateWithCitiesAsync(id);
                return Ok(state);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateState([FromBody] CreateStateDto createStateDto)
        {
            var state = await _stateService.CreateStateAsync(createStateDto);
            return CreatedAtAction(nameof(GetStateById), new { id = state.Id }, state);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateDto updateStateDto)
        {
            //if (id != updateStateDto.Id)
            //{
            //    return BadRequest("State ID mismatch");
            //}
            try
            {
                await _stateService.UpdateStateAsync(id, updateStateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteState(Guid id)
        {
            try
            {
                await _stateService.DeleteStateAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("ByCountry/{countryId}")]
        public async Task<IActionResult> GetStatesByCountry(Guid countryId)
        {
            try
            {
                var states = await _stateService.GetStatesByCountryIdAsync(countryId);
                return Ok(states);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        //[HttpGet("ByCity/{cityId}")]
        //public async Task<IActionResult> GetStatesByCity(Guid cityId)
        //{
        //    try
        //    {
        //        var states = await _stateService.GetStatesByCityAsync(cityId);
        //        return Ok(states);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}
    }

}
