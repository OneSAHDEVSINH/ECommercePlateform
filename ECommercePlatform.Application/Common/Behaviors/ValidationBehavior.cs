using ECommercePlatform.Application.Common.Models;
using FluentValidation;
using MediatR;
using System.Reflection;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;


namespace ECommercePlatform.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

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

                    // Formatting the error message
                    var formattedError = string.Join("; ",
                        errorMessages.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));
                    // Handle both generic AppResult<T> and non-generic AppResult responses
                    bool isGenericAppResult = typeof(TResponse).IsGenericType &&
                                             typeof(TResponse).GetGenericTypeDefinition() == typeof(AppResult<>);
                    bool isNonGenericAppResult = typeof(TResponse) == typeof(AppResult);

                    if (isGenericAppResult || isNonGenericAppResult)
                    {
                        // Get the appropriate Failure method
                        var resultType = typeof(TResponse);
                        MethodInfo? failureMethod = null;

                        if (isGenericAppResult)
                        {
                            failureMethod = resultType.GetMethod("Failure");
                        }
                        else // isNonGenericAppResult
                        {
                            failureMethod = typeof(AppResult).GetMethod("Failure", [typeof(string)]);
                        }

                        if (failureMethod != null)
                        {
                            var result = failureMethod.Invoke(null, [formattedError]);
                            if (result is not null)
                            {
                                return (TResponse)result;
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