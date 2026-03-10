using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public record CreateItemCommand(ItemType ItemType, string? Content, IFormFile? file, Guid PortfolioProjectId) : IRequest<Result<ItemDTO>>;
}