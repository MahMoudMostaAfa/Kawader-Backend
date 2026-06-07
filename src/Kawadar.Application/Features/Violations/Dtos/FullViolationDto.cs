using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Application.Features.Violations.Dtos
{
    public class FullViolationDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = "";
        public string ViolationEvidence { get; set; } = "";
        public ViolationType ViolationType { get; set; }

        public string ReferenceType { get; set; } = "";
        public string? RedirectUrl { get; set; }
        public ViolationStatus ViolationStatus { get; set; }
        public string ActionTaken { get; set; } = default!;
        public string NoteByAdmin { get; set; } = default!;
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedByUserName { get; set; } = "";
    }
}
