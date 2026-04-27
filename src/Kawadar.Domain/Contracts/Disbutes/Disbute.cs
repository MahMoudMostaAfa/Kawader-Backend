using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Domain.Contracts.Disbutes
{
    public class Disbute : AuditableEntity
    {
        public Guid ContractId { get; set; }
        public Guid RaisedById { get; set; }
        public string Reason { get; set; } = "";
        public DisbuteStatus Status { get; set; } = DisbuteStatus.Open;
        public string? Resolution { get; set; }
        public DateTime? ResolvedAt { get; set; }

        private Disbute(Guid ContractId, Guid RaisedById, string Reason) : base(new Guid())
        {
            this.ContractId = ContractId;
            this.RaisedById = RaisedById;
            this.Reason = Reason;
        }

        public static Result<Disbute> Create(Guid ContractId, Guid RaisedById, string Reason)
        {
            if (ContractId.Equals(Guid.Empty)) return DisbuteErrors.ContractIdIsRequired;
            if (RaisedById.Equals(Guid.Empty)) return DisbuteErrors.RaisedByIdIsRequired;
            if (string.IsNullOrEmpty(Reason)) return DisbuteErrors.ReasonIsRequired;

            var disbute = new Disbute(ContractId, RaisedById, Reason);
            return disbute;
        }

        public Result<Updated> Update(DisbuteStatus status, string? Resolution, DateTime? ResolvedAt)
        {
            Status = status;
            this.Resolution = Resolution;
            this.ResolvedAt = ResolvedAt;
            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}