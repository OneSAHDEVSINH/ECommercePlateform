using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ECommercePlatform.API.Middleware
{
    public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
    {
        private readonly AuthorizationOptions _options = options.Value;

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Check if policy exists in options first
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
                return policy;

            // If it's our permission policy
            if (policyName == "Permission")
            {
                var permissionPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                return permissionPolicy;
            }

            return null;
        }
    }
} 