using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.Modules;
using ECommercePlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModuleController(IUnitOfWork unitOfWork, IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private readonly IUnitOfWork _unitOfWork;

        [HttpGet]
        [HasPermission("Module", "View")]
        public async Task<IActionResult> GetAll()
        {
            var modules = await _unitOfWork.Modules.GetAllAsync();
            return Ok(modules);
        }

        [HttpGet("{id}")]
        [HasPermission("Module", "View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var module = await _unitOfWork.Modules.GetByIdAsync(id);
            if (module == null)
                return NotFound();

            return Ok(module);
        }

        // Additional endpoints would be similar to Role and User controllers


        [HttpGet]
        [HasPermission("Module", "View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllModulesWithPermissionsQuery());
            return Ok(result);
        }
    }
}