using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewStatistics
{
    public class GetReviewStatisticsQueryHandler(IUser user, IUsersRepository usersRepository,
        IIdentityService identityService, IReviewRepository reviewRepository) : IRequestHandler<GetReviewStatisticsQuery, Result<ReviewStatisticsDto>>
         
    {
        public async Task<Result<ReviewStatisticsDto>> Handle(GetReviewStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var UserProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (UserProfileResult.IsError) return UserProfileResult.Errors;

            var RequiredUserDto = await identityService.GetUserByUserNameAsync(request.userName);
            if (RequiredUserDto.IsError) return RequiredUserDto.Errors;

            var RequiredUserProfile = await usersRepository.GetUserProfileByUserIdAsync(RequiredUserDto.Value.Id);
            if (RequiredUserProfile.IsError) return RequiredUserProfile.Errors;

            var StatisticsResult = await reviewRepository.GetReviewsStatistics(RequiredUserProfile.Value.Id);
            if (StatisticsResult.IsError) return StatisticsResult.Errors;

            return StatisticsResult.Value;
        }
    }
}
