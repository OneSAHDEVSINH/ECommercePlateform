using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetPagedStates
{
    public class GetPagedStatesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPagedStatesQuery, AppResult<PagedResponse<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<PagedResponse<StateDto>>> Handle(GetPagedStatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedResponse = await _unitOfWork.States.GetPagedStateDtosAsync(
                    request,
                    request.CountryId,
                    request.ActiveOnly,
                    cancellationToken);

                return AppResult<PagedResponse<StateDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return AppResult<PagedResponse<StateDto>>.Failure(
                    $"An error occurred while retrieving states: {ex.Message}");
            }
        }
    }
}
