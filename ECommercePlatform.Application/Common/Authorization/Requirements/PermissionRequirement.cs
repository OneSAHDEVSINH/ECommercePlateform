// Application/Common/Authorization/Requirements/PermissionRequirement.cs
using Microsoft.AspNetCore.Authorization;

namespace ECommercePlatform.Application.Common.Authorization.Requirements
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Module { get; }
        public string Permission { get; }

        public PermissionRequirement(string module, string permission)
        {
            Module = module;
            Permission = permission;
        }
    }
}