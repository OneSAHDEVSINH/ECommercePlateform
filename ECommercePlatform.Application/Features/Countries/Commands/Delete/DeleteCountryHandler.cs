using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Delete
{
    public class DeleteCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCountryCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
                if (country == null)
                    return AppResult.Failure($"Country with ID {request.Id} not found.");

                var states = await _unitOfWork.States.GetStatesByCountryIdAsync(request.Id);
                if (states != null && states.Any())
                    return AppResult.Failure($"Cannot delete country with ID {request.Id} as it has associated states");

                await _unitOfWork.Countries.DeleteAsync(country);
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure(ex.Message);
            }
        }
    }
}
