using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleByRoute
{
    public class GetModuleByRouteHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleByRouteQuery, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(GetModuleByRouteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByRouteAsync(request.Route);
                if (module == null)
                    return AppResult<ModuleDto>.Failure($"Module with route '{request.Route}' not found.");

                // Map to DTO using the explicit operator
                var moduleDto = (ModuleDto)module;

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while retrieving the module: {ex.Message}");
            }
        }
    }
}