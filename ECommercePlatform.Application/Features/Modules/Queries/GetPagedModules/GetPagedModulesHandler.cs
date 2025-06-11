using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetPagedModules
{
    public class GetPagedModulesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedModulesQuery, AppResult<PagedResponse<ModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<ModuleDto>>> Handle(GetPagedModulesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.Modules.GetPagedModuleDtosAsync(
                    request,
                    request.ActiveOnly,
                    cancellationToken);

                return AppResult<PagedResponse<ModuleDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<ModuleDto>>.Failure($"An error occurred while retrieving modules: {ex.Message}");
            }
        }
    }
}