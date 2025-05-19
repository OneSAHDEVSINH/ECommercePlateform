using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.Handlers
{
    public class GetStateByIdHandler : IRequestHandler<GetStateByIdQuery, AppResult<StateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetStateByIdHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<AppResult<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            var state = await _unitOfWork.States.GetByIdAsync(request.Id);
            if (state == null)
            {
                return AppResult<StateDto>.Failure($"State with ID {request.Id} not found");
            }
            var stateDto = _mapper.Map<StateDto>(state);
            return AppResult<StateDto>.Success(stateDto);
        }
    }
}
