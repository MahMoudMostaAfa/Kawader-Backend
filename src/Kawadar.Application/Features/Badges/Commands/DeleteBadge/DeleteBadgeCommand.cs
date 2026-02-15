using Kawadar.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Application.Features.Badges.Commands.DeleteBadge
{
    public record DeleteBadgeCommand(Guid badgeId): IRequest<Result<Deleted>>;
}
