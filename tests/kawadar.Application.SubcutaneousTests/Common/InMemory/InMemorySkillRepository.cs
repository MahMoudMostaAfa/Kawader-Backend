using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemorySkillRepository : ISkillRepository
{
    public readonly List<Skill> Skills = [];
    public readonly List<FreelancerSkill> FreelancerSkills = [];
    public readonly List<PortfolioProjectSkill> ProjectSkills = [];

    public Task<Result<Success>> addAsync(Skill skill)
    {
        Skills.Add(skill);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Skill>> getByIdAsync(Guid Id)
    {
        var skill = Skills.FirstOrDefault(s => s.Id == Id);
        return Task.FromResult(skill is not null
            ? (Result<Skill>)skill
            : Error.NotFound("Skill.NotFound", $"Skill '{Id}' not found."));
    }

    public Task<Result<Skill>> getByNameAsync(string name)
    {
        var skill = Skills.FirstOrDefault(s => s.Name == name);
        return Task.FromResult(skill is not null
            ? (Result<Skill>)skill
            : Error.NotFound("Skill.NotFound", $"Skill '{name}' not found."));
    }

    public Result<Deleted> delete(Skill skill)
    {
        Skills.Remove(skill);
        return Result.Deleted;
    }

    public Task<IEnumerable<Skill>> getAllSkills()
    {
        return Task.FromResult(Skills.AsEnumerable());
    }

    public Task<Result<Success>> addSkillToFreelacner(List<FreelancerSkill> freelancerSkill)
    {
        FreelancerSkills.AddRange(freelancerSkill);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> addSkillToProject(List<PortfolioProjectSkill> projectSkill)
    {
        ProjectSkills.AddRange(projectSkill);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<List<string>> GetFreelancerSkillsByUserProfileId(Guid UserProfileId)
    {
        var skillIds = FreelancerSkills
            .Where(fs => fs.FreelancerId == UserProfileId && fs.SkillId.HasValue)
            .Select(fs => fs.SkillId!.Value)
            .ToHashSet();

        var skillNames = Skills
            .Where(s => skillIds.Contains(s.Id))
            .Select(s => s.Name)
            .ToList();

        // Also add custom skills
        var customSkills = FreelancerSkills
            .Where(fs => fs.FreelancerId == UserProfileId && !string.IsNullOrWhiteSpace(fs.CustomSkillName))
            .Select(fs => fs.CustomSkillName!)
            .ToList();

        skillNames.AddRange(customSkills);

        return Task.FromResult(skillNames);
    }

    public async Task<Result<IEnumerable<Skill>>> GetBySkillIds(List<Guid> skillIds)
    {
        await Task.CompletedTask;
        var idSet = skillIds.ToHashSet();
        var found = Skills.Where(s => idSet.Contains(s.Id)).ToList();
        return found;
    }

    public Task<Result<Deleted>> RemoveSkillFromFreelancer(string skillName, Guid freelancerId)
    {
        var skill = Skills.FirstOrDefault(s => s.Name == skillName);
        if (skill is null)
            return Task.FromResult<Result<Deleted>>(Error.NotFound("Skill.NotFound", $"Skill '{skillName}' not found."));

        var removed = FreelancerSkills.RemoveAll(fs => fs.FreelancerId == freelancerId && fs.SkillId == skill.Id);
        if (removed == 0)
            return Task.FromResult<Result<Deleted>>(Error.NotFound("FreelancerSkill.NotFound", "Freelancer does not have this skill."));

        return Task.FromResult<Result<Deleted>>(Result.Deleted);
    }

    public Task<List<string>> GetProjectSkillsByProjectId(Guid ProjectId)
    {
        var skillIds = ProjectSkills
            .Where(ps => ps.PortfolioProjectId == ProjectId)
            .Select(ps => ps.SkillId)
            .ToHashSet();

        var skillNames = Skills
            .Where(s => skillIds.Contains(s.Id))
            .Select(s => s.Name)
            .ToList();

        return Task.FromResult(skillNames);
    }

    public void Clear()
    {
        Skills.Clear();
        FreelancerSkills.Clear();
        ProjectSkills.Clear();
    }
}
