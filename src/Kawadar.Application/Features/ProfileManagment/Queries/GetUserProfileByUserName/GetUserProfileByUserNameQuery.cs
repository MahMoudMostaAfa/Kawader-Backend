using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfileByUserName;



public record GetUserProfileByUserNameQuery(string UserName) : IRequest<Result<UserProfileDto>>;