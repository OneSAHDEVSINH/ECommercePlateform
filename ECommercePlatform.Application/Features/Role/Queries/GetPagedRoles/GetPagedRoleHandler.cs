using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetPagedRoles
{
    public class GetPagedRolesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedRolesQuery, AppResult<PagedResponse<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<RoleDto>>> Handle(GetPagedRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.Roles.GetPagedRoleDtosAsync(
                    request,
                    request.ActiveOnly,
                    cancellationToken);

                return AppResult<PagedResponse<RoleDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<RoleDto>>.Failure($"An error occurred while retrieving paged roles: {ex.Message}");
            }
        }
    }
}