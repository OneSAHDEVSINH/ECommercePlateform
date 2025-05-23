using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesById
{
    public class GetStateByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStateByIdQuery, AppResult<StateDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var state = await _unitOfWork.States.GetByIdAsync(request.Id);
                if (state == null)
                    return AppResult<StateDto>.Failure($"State with this ID \"{request.Id}\" not found.");

                var stateDto = (StateDto)state;

                return AppResult<StateDto>.Success(stateDto);
            }
            catch (Exception ex)
            {
                return AppResult<StateDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
