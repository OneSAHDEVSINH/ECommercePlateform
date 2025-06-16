using Microsoft.AspNetCore.Authorization;

namespace ECommercePlatform.API.Middleware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string module, string permission)
        {
            Module = module;
            Permission = permission;
            //Policy = $"Permission:{module}:{permission}";
            Policy = "Permission";
        }
        // These will be populated into the requirement by the AuthorizationPolicyProvider
        public string Module { get; }
        public string Permission { get; }        
    }
}