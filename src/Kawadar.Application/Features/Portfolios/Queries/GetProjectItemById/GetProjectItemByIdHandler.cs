using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemById
{
    public class GetProjectItemByIdHandler(IUser user,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<GetProjectItemByIdQuery, Result<ItemDTO>>
    {
        public async Task<Result<ItemDTO>> Handle(GetProjectItemByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);

            if (result.IsError) return result.Errors;

            return result.Value.toDTO();
        }
    }
}
