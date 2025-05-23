using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Delete
{
    public class DeleteStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteStateCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var state = await _unitOfWork.States.GetByIdAsync(request.Id);
                if (state == null)                
                    return AppResult.Failure($"State with ID {request.Id} not found");
                
                var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(request.Id);
                if (cities != null && cities.Any())                
                    return AppResult.Failure($"Cannot delete state with ID {request.Id} as it has associated countries");
                
                await _unitOfWork.States.DeleteAsync(state);
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
