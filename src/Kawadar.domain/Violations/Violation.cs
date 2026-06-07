using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Domain.Violations
{
    public class Violation : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public string? ViolationEvidence { get; private set; } = default!;
        public ViolationType ViolationType { get; private set; }
        public Guid ReferenceId { get; private set; }
        public string ReferenceType { get; private set; }
        public string? RedirectUrl { get; private set; }
        public ViolationStatus ViolationStatus { get; private set; } = ViolationStatus.Pending;
        public string ActionTaken { get; private set; } = default!;
        public string NoteByAdmin { get; private set; } = default!;
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedBy { get; set; }     //refernce to the admin

        private Violation(Guid UserId, string ViolationEvidence, ViolationType ViolationType, Guid ReferenceId, string RedirectUrl, string ReferenceType) : base(new Guid())
        {
            this.UserId = UserId;
            this.ViolationEvidence = ViolationEvidence;
            this.ViolationType = ViolationType;
            this.ReferenceId = ReferenceId;
            this.ReferenceType = ReferenceType;
            this.RedirectUrl = RedirectUrl;
            ActionTaken = "Pending review";
            NoteByAdmin = string.Empty;
        }

        public static Result<Violation> Create(Guid UserId, string ViolationEvidence, ViolationType ViolationType
            , Guid ReferenceId, string RedirectUrl, string ReferenceType)
        {
            return new Violation(UserId, ViolationEvidence, ViolationType, ReferenceId, RedirectUrl, ReferenceType);
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
