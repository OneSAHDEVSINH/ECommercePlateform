using ECommercePlatform.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.States.Commands;

public record DeleteStateCommand(Guid Id) : IRequest<AppResult>;
