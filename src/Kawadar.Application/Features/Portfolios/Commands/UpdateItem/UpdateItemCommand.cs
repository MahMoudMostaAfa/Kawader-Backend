using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public record UpdateItemCommand(Guid Id, ItemType itemType, string? Content, IFormFile? Image) : IRequest<Result<Updated>>;
}
