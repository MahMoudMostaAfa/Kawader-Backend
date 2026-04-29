using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Api.Requests.Violations
{
    public class SolveViolationRequest
    {
        public ViolationStatus status { get; set; }
        public string? action { get; set; }
        public string noteByAdmin { get; set; } = "";
    }
}
