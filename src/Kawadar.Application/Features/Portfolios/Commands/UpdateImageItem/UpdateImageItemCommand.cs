
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateImageItem
{
    public record UpdateImageItemCommand(Guid Id, IFormFile Image):IRequest<Result<Updated>>;
}
