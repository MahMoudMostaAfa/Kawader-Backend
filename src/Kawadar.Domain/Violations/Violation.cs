using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Domain.Violations
{
    public class Violation : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string ViolationEvidence { get; set; } = default!;
        public ViolationType ViolationType { get; set; }
        public float severityScore { get; set; }
        public Guid ReferenceId { get; set; }
        public string ReferenceType { get; set; }
        public string? RedirectUrl { get; set; }
        public ViolationStatus ViolationStatus { get; set; } = ViolationStatus.Pending;
        public string ActionTaken { get; set; } = default!;
        public string NoteByAdmin { get; set; } = default!;
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedBy { get; set; }     //refernce to the admin

        private Violation(Guid UserId, string ViolationEvidence, ViolationType ViolationType, float severityScore, Guid ReferenceId, string RedirectUrl, string ReferenceType) : base(new Guid())
        {
            this.UserId = UserId;
            this.ViolationEvidence = ViolationEvidence;
            this.ViolationType = ViolationType;
            this.severityScore = severityScore;
            this.ReferenceId = ReferenceId;
            this.ReferenceType = ReferenceType;
            this.RedirectUrl = RedirectUrl;
        }

        public static Result<Violation> Create(Guid UserId, string ViolationEvidence, ViolationType ViolationType
            , float severityScore, Guid ReferenceId, string RedirectUrl, string ReferenceType)
        {
            return new Violation(UserId, ViolationEvidence, ViolationType, severityScore, ReferenceId, RedirectUrl, ReferenceType);
        }

        public Result<Updated> Solve(ViolationStatus status, string action, string noteByAdmin, Guid ResolvedBy)
        {
            ActionTaken = action;
            NoteByAdmin = noteByAdmin;
            this.ResolvedBy = ResolvedBy;
            ResolvedAt = DateTime.UtcNow;
            ViolationStatus = status;
            UpdatedAt = DateTime.UtcNow;

            return Result.Updated;
        }
    }
}
