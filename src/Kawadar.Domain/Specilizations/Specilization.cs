using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Specilizations
{
    public class Specilization: AuditableEntity
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        private Specilization(string name, bool isActive): base(Guid.NewGuid())
        {
            Name = name;
            IsActive = isActive;
        }

        public static Result<Specilization> Create(string name, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return SpecilizationErros.NameIsRequired;
            }

            var specilization = new Specilization(name, isActive);
            return specilization;
        }

        public Result<Updated> Update(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
            return Result.Updated;
        }
    }
}