

using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public record CreateItemCommand(ItemType ItemType, string Content, int DisplayOrder, Guid PortfolioProjectId) : IRequest<Result<ItemDTO>>;
}
