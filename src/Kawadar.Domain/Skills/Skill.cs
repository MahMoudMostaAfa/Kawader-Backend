using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Skills
{
    public class Skill: AuditableEntity
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public Guid CreatedBy { get; private set; }

        private Skill(string name, bool isActive, Guid createdBy) : base(Guid.NewGuid())
        {
            Name = name;
            IsActive = isActive;
            CreatedBy = createdBy;
        }

        public static Result<Skill> Create(string name, bool isActive, Guid createdBy)
        {
            if (string.IsNullOrEmpty(name))
            {
                return SkillErrors.NameIsRequired;
            }

            if(createdBy == Guid.Empty)
            {
                return SkillErrors.CreatorIdIsRequired;
            }

            var skill = new Skill(name, isActive, createdBy);
            return skill;
        }

        public Result<Updated> Update(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;

            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}
