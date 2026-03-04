using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetAdmins
{
    public record GetAdminsQuery() : IRequest<Result<List<AdminDto>>>;
}
