using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.OrderPortfolioProjects
{
    public class OrderPortfolioProjectsHandler(IUnitOfWork unitOfWork, IUser user,
        IPortfolioProjectRepository projectRepository, IUsersRepository usersRepository) : IRequestHandler<OrderPortfolioProjectsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(OrderPortfolioProjectsCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileresult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileresult.IsError) return userProfileresult.Errors;

            var projects = await projectRepository.GetAllByFreelancerId(userProfileresult.Value.Id);
            var projectsDictionary = projects.ToDictionary(x => x.Id);
            var orderDictionary = request.Order.ToDictionary(y => y.Id, y => y.DisplayOrder);

            foreach(var order in orderDictionary)
            {
                if(!projectsDictionary.TryGetValue(order.Key, out _))
                    return Error.NotFound("Project.NotFound", "Project not found");
                var project = projectsDictionary[order.Key];
                var updateResult = project.UpdateOrder(order.Value);
                if (updateResult.IsError) return updateResult.Errors;
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}