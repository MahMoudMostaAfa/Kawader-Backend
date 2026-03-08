using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetAdmins
{
    public record GetAdminsQuery(bool? IsOnline,
        bool? IsDeleted,
        int page,
        int pageSize,
        string sortBy) : IRequest<Result<PaginatedList<AdminDto>>>;
}
