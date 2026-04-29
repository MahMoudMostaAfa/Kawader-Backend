using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbuteById
{
    public record GetDisbuteByIdQuery(Guid Id) : IRequest<Result<fullDisbuteDto>>; 
}
