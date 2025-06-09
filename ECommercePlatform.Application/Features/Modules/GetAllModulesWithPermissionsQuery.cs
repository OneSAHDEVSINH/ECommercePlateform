using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules
{
    public class GetAllModulesWithPermissionsQuery : IRequest<List<ModuleDto>>
    {
    }
}