using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfile;


public record GetUserProfileQuery : IRequest<Result<UserProfileDto>>;