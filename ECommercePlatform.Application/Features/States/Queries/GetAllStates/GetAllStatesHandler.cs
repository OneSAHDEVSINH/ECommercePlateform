using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetAllStates
{
    public class GetAllStatesHandler : IRequestHandler<GetAllStatesQuery, AppResult<List<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllStatesHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<AppResult<List<StateDto>>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
        {
            var states = await _unitOfWork.States.GetAllAsync();
            var stateDtos = _mapper.Map<List<StateDto>>(states);
            return AppResult<List<StateDto>>.Success(stateDtos);
        }
    }
}
