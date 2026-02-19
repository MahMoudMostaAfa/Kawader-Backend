using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public record UpdateItemCommand(Guid Id, string Content) : IRequest<Result<Updated>>;
}
