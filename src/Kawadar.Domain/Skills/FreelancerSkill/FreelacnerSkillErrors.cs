using Kawadar.Domain.Common.Results;
using System.Reflection.Metadata.Ecma335;

namespace Kawadar.Domain.Skills.FreelancerSkill
{
    public class FreelacnerSkillErrors
    {
        public static Error FreelancerIdIsRequired => Error.Validation("FreelacnerSkill.FreelancerSkillIsRequired",
            "Freelancer Id can't be empty");
    }
}
