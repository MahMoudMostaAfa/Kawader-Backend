using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Application.Features.Contracts.Disbutes.Dtos
{
    public class AdminContractDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid ProposalId { get; set; }
        public Guid FreelancerId { get; set; }
        public string FreelancerUsername { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public string ClientUserName { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public decimal? OneTimeFixedPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? TotalMilestones { get; set; }
        public List<ContractMilestoneDto>? Milestones { get; set; } = new List<ContractMilestoneDto>();
    }
}
