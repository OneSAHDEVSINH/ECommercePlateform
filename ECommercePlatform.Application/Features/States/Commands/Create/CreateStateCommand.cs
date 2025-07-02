﻿using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Create
{
    public record CreateStateCommand(string Name, string Code) : IRequest<AppResult<StateDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; } = Name?.Trim() ?? string.Empty;
        public required string Code { get; init; } = Code?.Trim() ?? string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid CountryId { get; init; }
        public bool IsActive { get; init; } = true;
    }
}
