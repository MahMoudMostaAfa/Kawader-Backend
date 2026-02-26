using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.SetSpecilization
{
    public class SetSpecilizationHandler(IUser user, ISpecilizationRepository specilizationRepository
        , IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<SetSpecilizationCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SetSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userProfile = userProfileResult.Value;

            var specilizationResult = await specilizationRepository.GetByName(request.specilizationName);
            if (specilizationResult.IsError) return specilizationResult.Errors;

            userProfile.updateSpecilization(specilizationResult.Value.Id);
            await unitOfWork.SaveChangesAsync();
            return Result.Updated;
        }
    }
}
