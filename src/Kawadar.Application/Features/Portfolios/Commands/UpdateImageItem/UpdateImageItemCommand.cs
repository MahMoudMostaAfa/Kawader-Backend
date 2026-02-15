
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateImageItem
{
    public record UpdateImageItemCommand(Guid Id, IFormFile Image, int displayOrder):IRequest<Result<Updated>>;
}
