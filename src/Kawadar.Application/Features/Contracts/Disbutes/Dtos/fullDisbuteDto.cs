

using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Application.Features.Contracts.Disbutes.Dtos
{
    public class fullDisbuteDto
    {
        public Guid contractId { get; set; }
        public string RaisedByUserName { get; set; } = "";
        public DisbuteStatus status { get; set; }
        public string reason { get; set; } = "";
        public string? resolution { get; set; }
        public DateTime? ResolvedAt { get; set; }

    }
}
