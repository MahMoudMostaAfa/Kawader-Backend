using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;


namespace Kawadar.Domain.Portfolios.ProjectSkill
{
    public class PortfolioProjectSkill: AuditableEntity
    {
        //Foreign Keys
        public Guid PortfolioProjectId { get; private set; }
        public Guid SkillId { get; private set; }


        private PortfolioProjectSkill(Guid portfolioProjectId, Guid skillId): base(Guid.NewGuid())
        {
            PortfolioProjectId = portfolioProjectId;
            SkillId = skillId;
        }

        public static Result<PortfolioProjectSkill> Create(Guid portfolioProjectId, Guid skillId)
        {
            if(portfolioProjectId == Guid.Empty)
            {
                return ProjectSkillErrors.PortfolioProjectIdIsRequired;
            }

            if(skillId == Guid.Empty)
            {
                return ProjectSkillErrors.SkillIdIsRequired;
            }

            var ProjectSkill = new PortfolioProjectSkill(
                portfolioProjectId, skillId);

            return ProjectSkill;
        }
    }
}
