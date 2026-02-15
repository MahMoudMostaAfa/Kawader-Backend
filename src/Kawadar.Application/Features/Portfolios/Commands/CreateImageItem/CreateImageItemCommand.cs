using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateImageItem
{
    public record CreateImageItemCommand(ItemType ItemType, IFormFile Image, int DisplayOrder, Guid PortfolioProjectId): IRequest<Result<ItemDTO>>;
}
