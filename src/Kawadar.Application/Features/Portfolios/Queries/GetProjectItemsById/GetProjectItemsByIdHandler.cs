using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById
{
    public class GetProjectItemsByIdHandler(IUser user,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<GetProjectWithItemsByIdQuery, Result<List<ItemDTO>>>
    {
        public async Task<Result<List<ItemDTO>>> Handle(GetProjectWithItemsByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetWithItemsByProjectId(request.Id);
            if (result.IsError) return result.Errors;

            var Items = result.Value.Items;

            return Items.toDTOList();

        }
    }
}
