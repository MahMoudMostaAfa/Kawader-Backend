using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemById
{
    public record GetProjectItemByIdQuery(Guid Id) : IRequest<Result<ItemDTO>>;
}
