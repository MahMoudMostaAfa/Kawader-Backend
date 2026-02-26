
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Skills
{
    public class SkillErrors
    {
        public static Error NameIsRequired => Error.Validation("Skill.NameIsRequired",
            "Skill name can't be empty");

        public static Error CreatorIdIsRequired => Error.Validation("Skill.CreatorIdIsRequired",
            "The Creator Id is required to create a skill");
    }
}
