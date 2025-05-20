using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Common.Behaviors
{
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

        public AuditBehavior(
            ICurrentUserService currentUserService,
            ILogger<AuditBehavior<TRequest, TResponse>> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // For create operations
            if (request is IAuditableCreateRequest createRequest)
            {
                if (_currentUserService.IsAuthenticated)
                {
                    var userId = _currentUserService.UserId;
                    var userIdentifier = !string.IsNullOrEmpty(userId)
                        ? userId
                        : _currentUserService.Email ?? "system";

                    createRequest.CreatedBy = userIdentifier;
                    createRequest.CreatedOn = DateTime.Now;

                    _logger.LogInformation("Setting CreatedBy to {UserId} for {RequestType}",
                        userIdentifier, typeof(TRequest).Name);
                }
            }

            // For update operations
            if (request is IAuditableUpdateRequest updateRequest)
            {
                if (_currentUserService.IsAuthenticated)
                {
                    var userId = _currentUserService.UserId;
                    var userIdentifier = !string.IsNullOrEmpty(userId)
                        ? userId
                        : _currentUserService.Email ?? "system";

                    updateRequest.ModifiedBy = userIdentifier;
                    updateRequest.ModifiedOn = DateTime.Now;

                    _logger.LogInformation("Setting ModifiedBy to {UserId} for {RequestType}",
                        userIdentifier, typeof(TRequest).Name);
                }
            }

            // Continue with the next handler in the pipeline
            return await next();
        }
    }
}
