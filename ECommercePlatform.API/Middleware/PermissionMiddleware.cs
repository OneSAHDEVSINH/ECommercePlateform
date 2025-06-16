using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace ECommercePlatform.API.Middleware
{
    public class PermissionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Check if the endpoint has HasPermission attributes
            var permissions = endpoint.Metadata
                .OfType<HasPermissionAttribute>()
                .ToList();

            if (!permissions.Any())
            {
                await _next(context);
                return;
            }

            // For each permission required, check authorization
            foreach (var permission in permissions)
            {
                var requirement = new PermissionRequirement(permission.Module, permission.Permission);
                var authResult = await authorizationService.AuthorizeAsync(context.User, null, requirement);
                
                if (!authResult.Succeeded)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
} 