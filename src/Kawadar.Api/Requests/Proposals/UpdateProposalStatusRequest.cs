using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Api.Requests.Proposals;


public class UpdateProposalStatusRequest
{
  public JobProposalStatus NewProposalStatus { get; set; }
}