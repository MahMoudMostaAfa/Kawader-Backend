using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
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

        public async Task<Result<IEnumerable<Skill>>> GetBySkillIds(List<Guid> skillIds)
        {
            var skills = await appDbContext.Skills.Where(s => skillIds.Contains(s.Id)).ToListAsync();
            if (skills.Count != skillIds.Count) return Error.NotFound("Skill.NotFound", "One or more skills not found");
            return skills;
        }

        public async Task<List<string>> GetFreelancerSkillsByUserProfileId(Guid UserProfileId)
        {
            var PredefinedSkills = from fs in appDbContext.FreelacnerSkills
                                 join s in appDbContext.Skills on fs.SkillId equals s.Id
                                 where fs.FreelancerId == UserProfileId
                                 select s.Name;
            var customSkills = await appDbContext.FreelacnerSkills.Where(x => x.FreelancerId == UserProfileId && x.SkillType == SkillType.Custom)
                                                                  .Select(x => x.CustomSkillName).ToListAsync();
            var skills = await PredefinedSkills.ToListAsync();
            skills.AddRange(customSkills);
            return skills;
        }
    }
}
