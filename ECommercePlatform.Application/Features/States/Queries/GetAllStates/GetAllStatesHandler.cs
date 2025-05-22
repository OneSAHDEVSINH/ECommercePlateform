using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetAllStates
{
    public class GetAllStatesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllStatesQuery, AppResult<List<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<StateDto>>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var states = await _unitOfWork.States.GetAllAsync();
                var stateDtos = states.Select(state => (StateDto)state).ToList();
                return AppResult<List<StateDto>>.Success(stateDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<StateDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
