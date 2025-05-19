using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Handlers
{
    public class DeleteStateHandler : IRequestHandler<DeleteStateCommand, AppResult>
    {
        private readonly IStateRepository _stateRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteStateHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork)
        {
            _stateRepository = stateRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AppResult> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var state = await _stateRepository.GetByIdAsync(request.Id);
                if (state == null)
                {
                    return AppResult.Failure($"State with ID {request.Id} not found");
                }
                var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(request.Id);
                if (cities != null && cities.Any())
                {
                    return AppResult.Failure($"Cannot delete state with ID {request.Id} as it has associated countries");
                }
                await _unitOfWork.States.DeleteAsync(state);
                await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
