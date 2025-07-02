using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetAllModules
{
    public class GetAllModulesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllModulesQuery, AppResult<List<ModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<ModuleDto>>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var modules = request.ActiveOnly
                    ? await _unitOfWork.Modules.GetActiveModulesAsync()
                    : await _unitOfWork.Modules.GetAllAsync();

                var moduleDtos = modules.Select(m => (ModuleDto)m).ToList();

                return AppResult<List<ModuleDto>>.Success(moduleDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<ModuleDto>>.Failure($"An error occurred while retrieving modules: {ex.Message}");
            }
        }
    }
}