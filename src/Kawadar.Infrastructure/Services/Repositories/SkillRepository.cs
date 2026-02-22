using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class SkillRepository(AppDbContext appDbContext) : ISkillRepository
    {
        public async Task<Result<Success>> addAsync(Skill skill)
        {
            await appDbContext.Skills.AddAsync(skill);
            return Result.Success;
        }

        public async Task<Result<Success>> addSkillToFreelacner(List<FreelancerSkill> freelancerSkills)
        {
            await appDbContext.FreelacnerSkills.AddRangeAsync(freelancerSkills);
            return Result.Success;
        }

        public async Task<Result<Success>> addSkillToProject(List<PortfolioProjectSkill> projectSkills)
        {
            await appDbContext.ProjectSkills.AddRangeAsync(projectSkills);
            return Result.Success;
        }

        public Result<Deleted> delete(Skill skill)
        {
            appDbContext.Remove(skill);
            return Result.Deleted;
        }

        public async Task<IEnumerable<Skill>> getAllSkills()
        {
            var skills = await appDbContext.Skills.ToListAsync();
            return skills;
        }

        public async Task<Result<Skill>> getByIdAsync(Guid Id)
        {
            var skill = await appDbContext.Skills.FirstOrDefaultAsync(s => s.Id == Id);
            if (skill is null) return Error.NotFound("Skill.NotFound", "Skill not found");
            return skill;
        }

        public async Task<Result<Skill>> getByNameAsync(string name)
        {
            var skill = await appDbContext.Skills.FirstOrDefaultAsync(s => s.Name == name);
            if (skill is null) return Error.NotFound("Skill.NotFound", "Skill not found");
            return skill;
        }
    }
}
