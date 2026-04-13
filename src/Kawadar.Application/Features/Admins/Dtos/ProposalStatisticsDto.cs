using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Application.Features.Admins.Dtos
{
    public class ProposalStatisticsDto
    {
        public int totalProposals { get; set; }
        public int ProposalsThisMonth { get; set; }
        public Dictionary<JobProposalStatus, int>? DistributionBasedOnProposalStatus { get; set; }
    }
}
