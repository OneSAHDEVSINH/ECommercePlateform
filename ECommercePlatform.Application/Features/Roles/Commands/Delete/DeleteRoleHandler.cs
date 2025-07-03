using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Commands.Delete
{
    public class DeleteRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request.Id)
                    .Bind(async id =>
                    {
                        var role = await _unitOfWork.Roles.GetByIdAsync(id);
                        return role == null
                            ? Result.Failure<Role>($"Role with ID {id} not found.")
                            : Result.Success(role);
                    })
                    .Bind(async role =>
                    {
                        var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(role.Id);
                        return userRoles.Count != 0
                            ? Result.Failure<Role>("Cannot delete role. It is currently assigned to one or more users.")
                            : Result.Success(role);
                    })
                    .Tap(async role => await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id))
                    .Tap(async role => await _unitOfWork.Roles.DeleteAsync(role))
                    .Map(_ => AppResult.Success())
                    .Match(
                        success => success,
                        failure => AppResult.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while deleting the role: {ex.Message}");
            }
        }
    }
}


//var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
//if (role == null)
//    return AppResult.Failure($"Role with ID {request.Id} not found.");

//// Check if role is used by users
//var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(role.Id);
//if (userRoles.Count != 0)
//    return AppResult.Failure("Cannot delete role. It is currently assigned to one or more users.");

//// First delete all role permissions
//await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

//// Then delete the role itself
//await _unitOfWork.Roles.DeleteAsync(role);

//return AppResult.Success();