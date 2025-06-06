﻿using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public record UpdateStateCommand : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public Guid CountryId { get; init; }
        public bool IsActive { get; init; } = true;

        public UpdateStateCommand(string name, string code)
        {
            Name = name?.Trim() ?? string.Empty;
            Code = code?.Trim() ?? string.Empty;
        }
    }
}
