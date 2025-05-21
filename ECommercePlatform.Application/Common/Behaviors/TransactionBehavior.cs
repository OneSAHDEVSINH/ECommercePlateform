using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Only apply transaction for commands that are marked with ITransactionalRequest
            if (request is not ITransactionalBehavior)
            {
                // Not a transactional request, just pass through
                return await next();
            }

            try
            {
                // Log that we're starting a transaction
                _logger.LogInformation("Beginning transaction for {RequestType}", typeof(TRequest).Name);

                // Execute the next behavior in the pipeline (which will eventually execute the request handler)
                var response = await next();

                // If we get here without exceptions, commit the transaction
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Transaction completed successfully for {RequestType}", typeof(TRequest).Name);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during transaction for {RequestType}", typeof(TRequest).Name);
                throw; // Re-throw to maintain the exception flow
            }
        }
    }
}
