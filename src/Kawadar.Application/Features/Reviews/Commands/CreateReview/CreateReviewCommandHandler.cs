using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Reviews.Enums;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler(IUser user, IUsersRepository usersRepository
        , IJobsRepository jobsRepository, IReviewRepository reviewRepository, IIdentityService identityService
        , IUnitOfWork unitOfWork, IFreelancerVectorStore freelancerVectorStore) : IRequestHandler<CreateReviewCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var RevieweerResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (RevieweerResult.IsError) return RevieweerResult.Errors;

            var jobResult = await jobsRepository.GetJobBySlugAsync(request.jobSlug);
            if (jobResult.IsError) return jobResult.Errors;

            var RevieweeUserDto = await identityService.GetUserByUserNameAsync(request.RevieweeUserName);
            if (RevieweeUserDto.IsError) return RevieweeUserDto.Errors;

            var RevieweeResult = await usersRepository.GetUserProfileByUserIdAsync(RevieweeUserDto.Value.Id);
            if (RevieweeResult.IsError) return RevieweeResult.Errors;

            ReviewType reviewType;
            if (RevieweerResult.Value.ProfileType == ProfileType.Client && RevieweeResult.Value.ProfileType == ProfileType.Freelancer)
            {
                reviewType = ReviewType.ClientFreelancer;
            }
            else
            {
                reviewType = ReviewType.FreelancerClient;
            }
            var reviewResult = Review.Create(jobResult.Value.Id, RevieweerResult.Value.Id, RevieweeResult.Value.Id, reviewType, request.rating, request.comment);
            if (reviewResult.IsError) return reviewResult.Errors;

            await reviewRepository.AddReview(reviewResult.Value, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await freelancerVectorStore.UpdateFreelancerAsync(RevieweeResult.Value);

            return Result.Success;
        }
    }
}
