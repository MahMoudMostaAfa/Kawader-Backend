using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToProject
{
    public class AddSkillsToProjectHandler(IUser user, IPortfolioProjectRepository projectRepository
        ,ISkillRepository skillRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddSkillsToProjectCommand, Result<List<PortfolioProjectSkill>>>
    {
        public async Task<Result<List<PortfolioProjectSkill>>> Handle(AddSkillsToProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var projectExistsResult = await projectRepository.GetPortfolioProjectById(request.ProjectId);
            if (projectExistsResult.IsError) return projectExistsResult.Errors;

            List<PortfolioProjectSkill> projectSkills = new (); 

            foreach(var skill in request.skills)
            {
                var skillExistsResult = await skillRepository.getByIdAsync(skill.SkillId);
                if (skillExistsResult.IsError) return skillExistsResult.Errors;

                var projectSkillResult = PortfolioProjectSkill.Create(request.ProjectId, skill.SkillId);
                if (projectSkillResult.IsError) return projectSkillResult.Errors;
                projectSkills.Add(projectSkillResult.Value);
            }

            var addResult = await skillRepository.addSkillToProject(projectSkills);
            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync();

            return projectSkills;
        }
    }
}
