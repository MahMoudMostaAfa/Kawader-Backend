using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Application.Features.Badges.Mapper;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries.GetBadgeById
{
    public class GetBadgeByIdQueryHandler(IUser user, IBadgeRepository badgeRepository
        , IMapper mapper) : IRequestHandler<GetBadgeByIdQuery, Result<BadgeDTO>>
    {
        public async Task<Result<BadgeDTO>> Handle(GetBadgeByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await badgeRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;
            var badge = result.Value;
            var badgeDTO = mapper.Map<BadgeDTO>(badge);
            return badgeDTO;
        }
    }
}
