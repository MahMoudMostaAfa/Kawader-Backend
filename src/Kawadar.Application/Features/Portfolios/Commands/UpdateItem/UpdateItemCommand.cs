using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public record UpdateItemCommand(Guid Id, ItemType ItemType, string Content, int DisplayOrder) : IRequest<Result<Updated>>;
}
