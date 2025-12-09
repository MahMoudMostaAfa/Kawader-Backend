using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById
{
    public record GetProjectItemsByIdQuery(Guid Id) : IRequest<Result<List<ItemDTO>>>;
}
