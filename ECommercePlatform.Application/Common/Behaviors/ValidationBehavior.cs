using ECommercePlatform.Application.Common.Models;
using FluentValidation;
using MediatR;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;


namespace ECommercePlatform.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    // Group error messages by property
                    var errorMessages = failures
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

                    // For AppResult<T> responses, we can create a failure result with the error messages
                    if (typeof(TResponse).IsGenericType &&
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(AppResult<>))
                    {
                        // Formatting the error message
                        var formattedError = string.Join("; ",
                            errorMessages.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));

                        // Create a failed AppResult<T> with proper error message
                        // This uses reflection to create the appropriate AppResult<T> type
                        var resultType = typeof(TResponse);
                        var failureMethod = resultType.GetMethod("Failure");

                        if (failureMethod != null)
                        {
                            var result = failureMethod.Invoke(null, new object[] { formattedError });
                            if (result is not null)
                            {
                                return (TResponse)result; // Fix for CS8600 and CS8603
                            }
                        }
                    }

                    // If response cannot handle validation errors appropriately, then throw exception as fallback
                    throw new ValidationException($"Command Validation Errors for Type {typeof(TRequest).Name}");
                }
            }

            return await next(cancellationToken);
        }
    }
}