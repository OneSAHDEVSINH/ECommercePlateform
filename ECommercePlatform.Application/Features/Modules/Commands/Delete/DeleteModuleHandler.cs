using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Delete
{
    public class DeleteModuleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteModuleCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
                //if (module == null)
                //    return AppResult.Failure($"Module with ID {request.Id} not found.");

                //// Check if module has role permissions
                //var hasRolePermissions = await _unitOfWork.RolePermissions
                //    .AnyAsync(rp => rp.ModuleId == request.Id);

                //if (hasRolePermissions)
                //{
                //    return AppResult.Failure("Cannot delete module with associated role permissions. Please remove all role permissions for this module first.");
                //}

                //// Soft delete the module
                ////module.MarkAsDeleted(request.DeletedBy ?? "system"); // Need to add DeletedBy to command
                ////await _unitOfWork.Modules.UpdateAsync(module);
                //// Delete the module
                //await _unitOfWork.Modules.DeleteAsync(module);

                //return AppResult.Success();

                return await Result.Success(request.Id)
                    .Bind(async id =>
                    {
                        var module = await _unitOfWork.Modules.GetByIdAsync(id);
                        return module == null
                            ? Result.Failure<Module>($"Module with ID {id} not found.")
                            : Result.Success(module);
                    })
                    .Bind(async module =>
                    {
                        // Check if module has role permissions
                        var hasRolePermissions = await _unitOfWork.RolePermissions
                            .AnyAsync(rp => rp.ModuleId == module.Id);
                        return hasRolePermissions
                            ? Result.Failure<Module>("Cannot delete module with associated role permissions. Please remove all role permissions for this module first.")
                            : Result.Success(module);
                    })
                    .Tap(async module =>

                        // Soft delete the module
                        //module.MarkAsDeleted(request.DeletedBy ?? "system"); // Need to add DeletedBy to command
                        await _unitOfWork.Modules.DeleteAsync(module))
                    .Map(_ => AppResult.Success())
                    .Match(
                        success => success,
                        failure => AppResult.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while deleting the module: {ex.Message}");
            }
        }
    }
}