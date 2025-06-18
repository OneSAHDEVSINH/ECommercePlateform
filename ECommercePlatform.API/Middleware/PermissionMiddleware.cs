using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ECommercePlatform.API.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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
                var authResult = await authorizationService.AuthorizeAsync(
                    context.User,
                    null,
                    requirement);

                if (!authResult.Succeeded)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Access denied",
                        module = permission.Module,
                        permission = permission.Permission
                    });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
}