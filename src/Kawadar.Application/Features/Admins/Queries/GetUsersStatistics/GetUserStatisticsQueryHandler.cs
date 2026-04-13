using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetUsersStatistics
{
    public class GetUserStatisticsQueryHandler(IUser user, IUsersRepository usersRepository,
        IReviewRepository reviewRepository) : IRequestHandler<GetUserStatisticsQuery, Result<UserStatisticsDto>>
    {
        public async Task<Result<UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var UsersRoleCount = await usersRepository.GetUsersRoleCount();
            if (UsersRoleCount.IsError) return UsersRoleCount.Errors;

            var VerifiedUserCount = await usersRepository.GetVerifiedUserCount();
            var NewUserThisMonth = await usersRepository.GetNewUsersThisMonth();
            var ratingStatistics = await reviewRepository.GetRatingStatistics();

            var userStatisticsDto = new UserStatisticsDto
            {
                usersCount = UsersRoleCount.Value.TotalCount,
                FreelancersCount = UsersRoleCount.Value.FreelancersCount,
                ClientsCount = UsersRoleCount.Value.ClientsCount,
                VerifiedUserCount = VerifiedUserCount,
                NewUsersThisMonth = NewUserThisMonth.Value,
                averageUserRating = ratingStatistics.Value.averageRating,
                HighestUserRating = ratingStatistics.Value.HighestRated,
                LowestUserRating = ratingStatistics.Value.LowestRated
            };

            return userStatisticsDto;
        }
    }
}
