using Microsoft.AspNetCore.Authorization;

namespace ECommercePlatform.API.Middleware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public string Module { get; }
        public string Permission { get; }

        public HasPermissionAttribute(string module, string permission)
        {
            Module = module;
            Permission = permission;
            Policy = $"Permission:{module}:{permission}";
        }
    }
}