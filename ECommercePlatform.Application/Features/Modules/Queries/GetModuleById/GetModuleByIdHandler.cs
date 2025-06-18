using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleById
{
    public class GetModuleByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleByIdQuery, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
                if (module == null)
                    return AppResult<ModuleDto>.Failure($"Module with ID {request.Id} not found.");

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