using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Presentation.Middleware
{
    public class SwaggerExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SwaggerExceptionMiddleware> _logger;

        public SwaggerExceptionMiddleware(RequestDelegate next, ILogger<SwaggerExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only intercept swagger.json requests
            if (context.Request.Path.Value?.Contains("/swagger/v1/swagger.json") == true)
            {
                _logger.LogInformation("Swagger JSON generation started");

                // Store the original response body stream
                var originalBodyStream = context.Response.Body;

                try
                {
                    // Create a new memory stream to hold the response
                    using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;
                    
                    try
                    {
                        // Continue down the pipeline
                        await _next(context);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception during Swagger JSON generation: {ErrorMessage}", ex.Message);
                        if (ex.InnerException != null)
                        {
                            _logger.LogError("Inner exception: {InnerException}", ex.InnerException.Message);
                        }
                        
                        // Return a custom error response
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 500;
                        var errorJson = "{ \"error\": \"Error generating Swagger documentation\", \"details\": \"" + ex.Message.Replace("\"", "'") + "\" }";
                        var errorBytes = Encoding.UTF8.GetBytes(errorJson);
                        responseBody.SetLength(0);
                        await responseBody.WriteAsync(errorBytes, 0, errorBytes.Length);
                    }
                    
                    // Copy the contents of the new memory stream to the original response stream
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                finally
                {
                    // Restore the original response body stream
                    context.Response.Body = originalBodyStream;
                }
            }
            else
            {
                // For non-swagger requests, just continue the pipeline
                await _next(context);
            }
        }
    }

    // Extension method used to add the middleware
    public static class SwaggerExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerExceptionMiddleware>();
        }
    }
} 