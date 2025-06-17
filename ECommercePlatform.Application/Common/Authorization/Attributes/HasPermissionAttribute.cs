// Application/Authorization/Attributes/HasPermissionAttribute.cs
using Microsoft.AspNetCore.Authorization;

namespace ECommercePlatform.Application.Common.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string module, string permission)
        {
            Module = module;
            Permission = permission;
            Policy = $"Permission:{module}:{permission}";
        }

        public string Module { get; }
        public string Permission { get; }
    }
}