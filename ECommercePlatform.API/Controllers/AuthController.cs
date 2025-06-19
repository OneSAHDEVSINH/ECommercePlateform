using ECommercePlatform.Application.Features.Auth.Commands.Login;
using ECommercePlatform.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if (command == null)
            {
                return BadRequest(new { message = "Invalid request body" });
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                // Return 401 for authentication failures
                if (result.Error.Contains("Invalid email or password") ||
                    result.Error.Contains("User account is inactive"))
                {
                    return Unauthorized(new { message = result.Error });
                }

                // Return 403 for authorization failures (no roles)
                if (result.Error.Contains("don't have any assigned roles"))
                {
                    return Forbid(result.Error);
                }

                // Return 400 for other errors
                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        //{
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSuccess)
        //        return BadRequest(new { message = result.Error });

        //    return Ok(result.Value);
        //}

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result.Value);
        }

        //private readonly IAuthService _authService;

        //public AuthController(IAuthService authService)
        //{
        //    _authService = authService;
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        //{
        //    try
        //    {
        //        if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        //            return BadRequest(new { message = "Email and password are required" });

        //        var result = await _authService.LoginAsync(loginDto);
        //        return Ok(result);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return Unauthorized(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
    }
}