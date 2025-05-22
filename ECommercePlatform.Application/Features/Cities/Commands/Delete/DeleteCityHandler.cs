using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Delete
{
    public class DeleteCityHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCityCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
                if (city == null)
                {
                    return AppResult.Failure($"City with ID {request.Id} not found");
                }

                await _unitOfWork.Cities.DeleteAsync(city);
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
