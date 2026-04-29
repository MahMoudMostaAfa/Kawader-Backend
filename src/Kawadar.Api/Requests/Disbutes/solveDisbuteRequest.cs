using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Api.Requests.Disbutes
{
    public class solveDisbuteRequest
    {
        public DisbuteStatus status { get; set; }
        public string resolution { get; set; } = "";
    }
}
