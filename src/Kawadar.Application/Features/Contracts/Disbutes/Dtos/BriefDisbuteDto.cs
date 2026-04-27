using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Application.Features.Contracts.Disbutes.Dtos
{
    public class BriefDisbuteDto
    {
        public string RaisedByUserName { get; set; } = "";
        public DisbuteStatus status { get; set; }
        public string reason { get; set; } = "";
    }
}
