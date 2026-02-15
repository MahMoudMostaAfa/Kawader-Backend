using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectView;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateView
{
    public class CreateViewCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IProjectViewRepository projectView, IUsersRepository usersRespository) : IRequestHandler<CreateViewCommand, Result<PortfolioProjectView>>
    {
        public async Task<Result<PortfolioProjectView>> Handle(CreateViewCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfile = await usersRespository.GetUserProfileByUserIdAsync(userId);
            if (userProfile.IsError) return userProfile.Errors;

            var result = PortfolioProjectView.Create(request.ProjectId, userProfile.Value.Id);
            if (result.IsError) return result.Errors;

            var addResult = await projectView.addAsync(result.Value);
            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value;
        }
    }
}
