using ECommercePlatform.Application.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ECommercePlatform.Application.Common.Authorization.Policies
{
    public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
    {
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Check if policy exists in options first
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
                return policy;

            // Check if this is a permission policy
            if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = policyName.Split(':');
                if (parts.Length == 3)
                {
                    var module = parts[1];
                    var permission = parts[2];

                    var policyBuilder = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermissionRequirement(module, permission));

                    return policyBuilder.Build();
                }
            }
            return null;
        }
    }
}