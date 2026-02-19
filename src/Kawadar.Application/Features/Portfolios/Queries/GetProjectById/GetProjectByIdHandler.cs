using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectById
{
    public class GetProjectByIdHandler(IUser user, IPortfolioProjectRepository projectRepository
        , IMapper mapper) : IRequestHandler<GetProjectByIdQuery, Result<ProjectDTO>>
    {
        public async Task<Result<ProjectDTO>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetPortfolioProjectById(request.Id);
            if (result.IsError) return result.Errors;

            var projectDTO = mapper.Map<ProjectDTO>(result.Value);

            return projectDTO;
        }
    }
}
