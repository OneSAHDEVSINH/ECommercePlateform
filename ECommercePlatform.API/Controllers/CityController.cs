using ECommercePlatform.Application.Features.Cities.Commands.Create;
using ECommercePlatform.Application.Features.Cities.Commands.Update;
using ECommercePlatform.Application.Features.Cities.Queries;
using ECommercePlatform.Application.Interfaces.IState;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly IMediator _mediator;

        public CityController(IStateService stateService, IMediator mediator)
        {
            _stateService = stateService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var result = await _mediator.Send(new GetAllCitiesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(Guid id)
        {
            var result = await _mediator.Send(new GetCityByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetCityById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityCommand command)
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
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var result = await _mediator.Send(new DeleteCityCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpGet("ByState/{stateId}")]
        public async Task<IActionResult> GetCitiesByState(Guid stateId)
        {
            var result = await _mediator.Send(new GetCitiesByStateQuery(stateId));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAllCities()
    //{
    //    var cities = await _cityService.GetAllCitiesAsync();
    //    return Ok(cities);
    //}

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetCityById(Guid id)
    //{
    //    try
    //    {
    //        var city = await _cityService.GetCityByIdAsync(id);
    //        return Ok(city);
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(new { message = ex.Message });
    //    }
    //}

    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> CreateCity([FromBody] CreateCityDto createCityDto)
    //{
    //    try
    //    {
    //        var city = await _cityService.CreateCityAsync(createCityDto);
    //        return CreatedAtAction(nameof(GetCityById), new { id = city.Id }, city);
    //    }
    //    catch (DuplicateResourceException ex)
    //    {
    //        // Return 409 Conflict with the error message
    //        return Conflict(new { message = ex.Message });
    //    }
    //}

    //[HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto updateCityDto)
    //{
    //    try
    //    {
    //        await _cityService.UpdateCityAsync(id, updateCityDto);
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
    //public async Task<IActionResult> DeleteCity(Guid id)
    //{
    //    try
    //    {
    //        await _cityService.DeleteCityAsync(id);
    //        return NoContent();
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(new { message = ex.Message });
    //    }
    //}

    //[HttpGet("ByState/{stateId}")]
    //public async Task<IActionResult> GetCitiesByState(Guid stateId)
    //{
    //    try
    //    {
    //        var cities = await _cityService.GetCitiesByStateIdAsync(stateId);
    //        return Ok(cities);
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(new { message = ex.Message });
    //    }
    //}
}

