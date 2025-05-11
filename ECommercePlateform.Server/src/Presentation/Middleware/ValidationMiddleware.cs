using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Presentation.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Store the original body stream position
            var originalBodyStream = context.Response.Body;
            
            try
            {
                // Continue down the pipeline
                await _next(context);

                // If we have a validation problem (status code 400), format the response
                if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    // Check if this is an API controller
                    var endpoint = context.GetEndpoint();
                    if (endpoint?.Metadata?.GetMetadata<ApiControllerAttribute>() != null)
                    {
                        // Try to read the response body (which might contain validation errors)
                        if (context.Items.TryGetValue("ValidationErrors", out var validationErrors) && validationErrors != null)
                        {
                            var errors = validationErrors as Dictionary<string, string[]>;
                            
                            if (errors != null && errors.Count > 0)
                            {
                                var formattedErrors = errors
                                    .SelectMany(e => e.Value.Select(error => new { Field = e.Key, Message = error }))
                                    .ToList();

                                var response = new
                                {
                                    StatusCode = context.Response.StatusCode,
                                    Message = "Validation Failed",
                                    Errors = formattedErrors
                                };

                                context.Response.ContentType = "application/json";
                                var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                                var json = JsonSerializer.Serialize(response, jsonOptions);
                                await context.Response.WriteAsync(json);
                            }
                        }
                    }
                }
            }
            finally
            {
                // Ensure any cleanup happens if necessary
            }
        }
    }

    // Extension method to register the middleware
    public static class ValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }
} 