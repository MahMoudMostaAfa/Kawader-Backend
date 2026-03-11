using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetUsers
{
    public record GetUserProfilesQuery(bool? IsDeleted,
        bool? IsBanned,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        int page,
        int pageSize,
        string sortBy) : IRequest<Result<PaginatedList<BriefUserProfileDto>>>;
}
