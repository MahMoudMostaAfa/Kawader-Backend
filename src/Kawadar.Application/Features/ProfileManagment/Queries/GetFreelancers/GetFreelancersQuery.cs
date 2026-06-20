using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancers
{
    public record GetFreelancersQuery(
        string? search,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        float? averageRating,
        List<Guid>? skillsIds,
        int page,
        int pageSize,
        string sortBy) : IRequest<Result<PaginatedList<BriefFreelancerDto>>>;
}
