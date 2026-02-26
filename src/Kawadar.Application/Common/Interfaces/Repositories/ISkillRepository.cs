using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface ISkillRepository
    {
        public Task<Result<Success>> addAsync(Skill skill);
        public Task<Result<Skill>> getByIdAsync(Guid Id);
        public Task<Result<Skill>> getByNameAsync(string name);
        public Result<Deleted> delete(Skill skill);
        public Task<IEnumerable<Skill>> getAllSkills();
        public Task<Result<Success>> addSkillToFreelacner(List<FreelancerSkill> freelancerSkill);
        public Task<Result<Success>> addSkillToProject(List<PortfolioProjectSkill> projectSkill);

    }
}