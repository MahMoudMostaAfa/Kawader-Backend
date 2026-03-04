using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetUsers
{
    public record GetUserProfilesQuery() : IRequest<Result<List<UserProfileDto>>>;
}
