using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.OrderProjectItems
{
    public record OrderProjectItemsCommand(Guid ProjectId,List<ItemOrderDTO> Order): IRequest<Result<Updated>>;
}