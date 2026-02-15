using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Specilizations
{
    public class SpecilizationErros
    {
        public static Error NameIsRequired => Error.Validation("Specilization.NameIsRequired",
            "Name can't be empty");
    }
}
