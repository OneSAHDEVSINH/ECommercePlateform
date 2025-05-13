using Microsoft.AspNetCore.Builder;

namespace ECommercePlateform.Server.Presentation.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }

        public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }
} 