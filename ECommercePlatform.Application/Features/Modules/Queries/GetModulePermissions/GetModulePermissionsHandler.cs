using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModulePermissions
{
    public class GetModulePermissionsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModulePermissionsQuery, AppResult<List<RolePermissionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<RolePermissionDto>>> Handle(GetModulePermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var permissions = await _unitOfWork.RolePermissions
                    .GetByModuleIdAsync(request.ModuleId);

                var permissionDtos = permissions.Select(p => (RolePermissionDto)p).ToList();

                return AppResult<List<RolePermissionDto>>.Success(permissionDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<RolePermissionDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
