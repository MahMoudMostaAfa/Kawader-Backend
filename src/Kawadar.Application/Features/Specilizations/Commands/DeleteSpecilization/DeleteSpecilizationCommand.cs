using Kawadar.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Application.Features.Specilizations.Commands.DeleteSpecilization
{
    public record DeleteSpecilizationCommand(Guid Id) : IRequest<Result<Deleted>>;
}
