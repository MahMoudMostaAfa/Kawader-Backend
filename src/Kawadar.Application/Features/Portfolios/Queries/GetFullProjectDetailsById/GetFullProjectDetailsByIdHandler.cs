using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById
{
    public class GetFullProjectDetailsByIdHandler(IUser user, IPortfolioProjectRepository projectRepository,
        ISkillRepository skillRepository, IMapper mapper) : IRequestHandler<GetFullProjectDetailsByIdQuery, Result<FullProjectDto>>
    {
        public async Task<Result<FullProjectDto>> Handle(GetFullProjectDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetWithItemsByProjectId(request.Id);
            if (result.IsError) return result.Errors;

            var Items = result.Value.Items;
            var itemDTOs = mapper.Map<List<ItemDTO>>(Items);

            var skills = await skillRepository.GetProjectSkillsByProjectId(request.Id);

            var project = mapper.Map<FullProjectDto>((result.Value, skills, itemDTOs));
            return project;

        }
    }
}
