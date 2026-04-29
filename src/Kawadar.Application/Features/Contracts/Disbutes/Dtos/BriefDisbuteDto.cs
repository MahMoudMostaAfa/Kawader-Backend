using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Application.Features.Contracts.Disbutes.Dtos
{
    public class BriefDisbuteDto
    {
        public Guid Id { get; set; }
        public string RaisedByUserName { get; set; } = "";
        public DisbuteStatus status { get; set; }
        public string reason { get; set; } = "";
    }
}
