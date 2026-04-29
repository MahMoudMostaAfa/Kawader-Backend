using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.RaiseDisbute
{
    public class RaiseDisbuteCommandHandler(IUser user, IUsersRepository usersRepository,
        IDisbuteRepository disbuteRepository, IUnitOfWork unitOfWork) : IRequestHandler<RaiseDisbuteCommand, Result<Created>>
    {
        public async Task<Result<Created>> Handle(RaiseDisbuteCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var disbuteResult = Disbute.Create(request.ContractId, userProfileResult.Value.Id, request.reason);
            if (disbuteResult.IsError) return disbuteResult.Errors;

            await disbuteRepository.AddDisbute(disbuteResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Created;
        }
    }
}
