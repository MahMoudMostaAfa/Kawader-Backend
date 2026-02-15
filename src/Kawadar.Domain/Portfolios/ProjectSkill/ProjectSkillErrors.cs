

using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.ProjectSkill
{
    public class ProjectSkillErrors
    {
        public static Error PortfolioProjectIdIsRequired => Error.Validation("PortfolioProjectSkill.PortfolioProjectIdIsRequired",
            "Portfolio Project Id is required to create ProjectSkill");

        public static Error SkillIdIsRequired => Error.Validation("PortfolioProjectSkill.SkillIdIsRequired",
            "Skill Id is required to create ProjectSkill");
    }
}
