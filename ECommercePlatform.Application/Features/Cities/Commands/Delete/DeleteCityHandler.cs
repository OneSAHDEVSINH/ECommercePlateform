using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.ICity;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Delete
{
    public class DeleteCityHandler : IRequestHandler<DeleteCityCommand, AppResult>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCityHandler(ICityRepository cityRepository, IUnitOfWork unitOfWork)
        {
            _cityRepository = cityRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AppResult> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var city = await _cityRepository.GetByIdAsync(request.Id);
                if (city == null)
                {
                    return AppResult.Failure($"City with ID {request.Id} not found");
                }

                await _unitOfWork.Cities.DeleteAsync(city);
                //await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
