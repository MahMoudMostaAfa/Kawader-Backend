using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Queries.GetAllSkills
{
    public class GetAllSkillsHandler(IUser user, ISkillRepository skillRepository) : IRequestHandler<GetAllSkillsQuery, Result<List<Skill>>>
    {
        public async Task<Result<List<Skill>>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var skills = await skillRepository.getAllSkills();
            var activeSkills = skills.Where(x => x.IsActive == true);

            return activeSkills.ToList();
        }
    }
}
