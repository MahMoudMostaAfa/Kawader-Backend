

using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Application.Features.Violations.Dtos
{
    public class BriefViolationDto
    {
        public string userName { get; set; } = "";
        public ViolationType violationType { get; set; }
        public ViolationStatus violationStatus { get; set; }
        public float severityScore { get; set; }
        public string RefernceType { get; set; } = "";
    }
}
