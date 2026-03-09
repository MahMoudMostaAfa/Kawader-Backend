using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewsByUserProfileId
{
    public class GetReviewsByUserNameQueryHandler(IUser user, IUsersRepository usersRepository,
        IIdentityService identityService, IReviewRepository reviewRepository, IMapper mapper) : IRequestHandler<GetReviewsByUserNameQuery, Result<PaginatedList<ReviewDto>>>
    {
        public async Task<Result<PaginatedList<ReviewDto>>> Handle(GetReviewsByUserNameQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var RevieweeUserResult = await identityService.GetUserByUserNameAsync(request.userName);
            if (RevieweeUserResult.IsError) return RevieweeUserResult.Errors;

            var RevieweeProfileReuslt = await usersRepository.GetUserProfileByUserIdAsync(RevieweeUserResult.Value.Id);
            if (RevieweeProfileReuslt.IsError) return RevieweeProfileReuslt.Errors;

            var reviews = await reviewRepository.GetReviewsByUserProfileId(request.rating, request.page, request.pageSize, request.sortBy, RevieweeProfileReuslt.Value.Id);

            var UserProfileIds = reviews.Items.Select(x => x.RevieweeId);

            var UserProfiles = await usersRepository.GetUsersbyIds(UserProfileIds);
            if (UserProfiles.IsError) return UserProfiles.Errors;

            var UserIds = UserProfiles.Value.Select(x => x.UserId);

            var UserDtos = await identityService.GetUsersByIds(UserIds);
            if (UserDtos.IsError) return UserDtos.Errors;

            var ReviewDtos = reviews.Items
                .Zip(UserDtos.Value, (review, userDto) => (review, userDto))
                .Select(pair => mapper.Map<ReviewDto>(pair))
                .ToList();

            return new PaginatedList<ReviewDto>(ReviewDtos, reviews.TotalCount, request.page, request.pageSize);
        }
    }
}
