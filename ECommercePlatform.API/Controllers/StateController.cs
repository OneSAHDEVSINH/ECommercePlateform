using ECommercePlatform.Application.Features.States.Commands.Create;
using ECommercePlatform.Application.Features.States.Commands.Update;
using ECommercePlatform.Application.Features.States.Queries;
using ECommercePlatform.Application.Interfaces.IState;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly IMediator _mediator;

        public StateController(IStateService stateService, IMediator mediator)
        {
            _stateService = stateService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStates()
        {
            var result = await _mediator.Send(new GetAllStatesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStateById(Guid id)
        {
            var result = await _mediator.Send(new GetStateByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
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
        public async Task<IActionResult> CreateState([FromBody] CreateStateCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetStateById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "Id in the URL does not match the Id in the request body" });
            }

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : Conflict(new { message = result.Error });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteState(Guid id)
        {
            var result = await _mediator.Send(new DeleteStateCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpGet("ByCountry/{countryId}")]
        public async Task<IActionResult> GetStatesByCountry(Guid countryId)
        {
            var result = await _mediator.Send(new GetStatesByCountryQuery(countryId));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllStates()
        //{
        //    var states = await _stateService.GetAllStatesAsync();
        //    return Ok(states);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetStateById(Guid id)
        //{
        //    try
        //    {
        //        var state = await _stateService.GetStateByIdAsync(id);
        //        return Ok(state);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        //[HttpGet("{id}/cities")]
        //public async Task<IActionResult> GetStateWithCities(Guid id)
        //{
        //    try
        //    {
        //        var state = await _stateService.GetStateWithCitiesAsync(id);
        //        return Ok(state);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateState([FromBody] CreateStateDto createStateDto)
        //{
        //    try
        //    {
        //        var state = await _stateService.CreateStateAsync(createStateDto);
        //        return CreatedAtAction(nameof(GetStateById), new { id = state.Id }, state);
        //    }
        //    catch (DuplicateResourceException ex)
        //    {
        //        // Return 409 Conflict with the error message
        //        return Conflict(new { message = ex.Message });
        //    }
        //}

        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateDto updateStateDto)
        //{
        //    //if (id != updateStateDto.Id)
        //    //{
        //    //    return BadRequest("State ID mismatch");
        //    //}
        //    try
        //    {
        //        await _stateService.UpdateStateAsync(id, updateStateDto);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (DuplicateResourceException ex)
        //    {
        //        // Return 409 Conflict with the error message
        //        return Conflict(new { message = ex.Message });
        //    }
        //}

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteState(Guid id)
        //{
        //    try
        //    {
        //        await _stateService.DeleteStateAsync(id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        //[HttpGet("ByCountry/{countryId}")]
        //public async Task<IActionResult> GetStatesByCountry(Guid countryId)
        //{
        //    try
        //    {
        //        var states = await _stateService.GetStatesByCountryIdAsync(countryId);
        //        return Ok(states);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

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
