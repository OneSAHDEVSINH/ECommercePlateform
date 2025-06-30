using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Create
{
    public class CreateModuleHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateModuleCommand, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Modules.EnsureNameRouteIconDPAreUniqueAsync(request.Name, request.Route, request.Icon!, request.DisplayOrder)
                .Map(_ =>
                    {
                        var module = Domain.Entities.Module.Create(
                            request.Name,
                            request.Description ?? string.Empty,
                            request.Route,
                            request.Icon ?? string.Empty,
                            request.DisplayOrder
                    );

                        if (!request.IsActive)
                        {
                            module.SetActive(false, request.CreatedBy ?? "system");
                        }

                        return module;
                    })
                .Tap(module => _unitOfWork.Modules.AddAsync(module))
                .Map(module => AppResult<ModuleDto>.Success((ModuleDto)module));

                return result.IsSuccess
                    ? result.Value
                    : AppResult<ModuleDto>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while creating the module: {ex.Message}");
            }
        }
    }
}



//// Validate uniqueness
//var uniqueResult = await _unitOfWork.Modules.EnsureNameRouteIconDPAreUniqueAsync(request.Name, request.Route, request.Icon!, request.DisplayOrder);
//if (uniqueResult.IsFailure)
//    return AppResult<ModuleDto>.Failure(uniqueResult.Error);

//// Create module entity
//var module = Domain.Entities.Module.Create(
//    request.Name,
//    request.Description ?? string.Empty,
//    request.Route,
//    request.Icon ?? string.Empty,
//    request.DisplayOrder
//);

//// Use the SetActive method from BaseEntity
//if (!request.IsActive)
//{
//    module.SetActive(false, request.CreatedBy ?? "system");
//}

//// Add module to database
//await _unitOfWork.Modules.AddAsync(module);

//// Map to DTO and return
//var moduleDto = (ModuleDto)module;

//return AppResult<ModuleDto>.Success(moduleDto);