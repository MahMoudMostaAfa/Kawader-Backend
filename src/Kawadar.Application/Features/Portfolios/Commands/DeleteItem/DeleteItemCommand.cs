using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteItem
{
    public record DeleteItemCommand(Guid Id) : IRequest<Result<Deleted>>;
}
