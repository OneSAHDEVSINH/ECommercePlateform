using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommercePlatform.API.Middleware
{
    public class ValidationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Continue down the pipeline
            await _next(context);

            // If we have a validation problem (status code 400) and it's from ModelState validation, format the response
            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                // Check if this is a validation error response
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<ApiControllerAttribute>() != null)
                {
                    // This is an API controller, we can handle validation errors
                    var problemDetailsFeature = context.Features.Get<IProblemDetailsService>(); // Updated to use IProblemDetailsService
                    if (problemDetailsFeature != null)
                    {
                        // Check if this is a validation problem
                        var validationProblem = problemDetailsFeature as ValidationProblemDetails;
                        if (validationProblem != null)
                        {
                            // Format the validation error in a consistent way
                            var errors = validationProblem.Errors
                                .SelectMany(e => e.Value.Select(error => new { Field = e.Key, Message = error }))
                                .ToList();

                            var response = new
                            {
                                context.Response.StatusCode,
                                Message = "Validation Failed",
                                Errors = errors
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
    }
}