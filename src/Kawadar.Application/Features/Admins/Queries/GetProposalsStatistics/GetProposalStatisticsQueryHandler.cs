using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetProposalsStatistics
{
    public class GetProposalStatisticsQueryHandler(IUser user,
        IProposalsRepository proposalsRepository) : IRequestHandler<GetProposalStatisticsQuery, Result<ProposalStatisticsDto>>
    {
        public async Task<Result<ProposalStatisticsDto>> Handle(GetProposalStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var ProposalCount = await proposalsRepository.GetProposalsCount();
            var ProposalThisMonth = await proposalsRepository.GetNumberOfProposalsThisMonth();

            var ProposalDistributionBasedOnStatus = await proposalsRepository.GetDistributionBasedOnStatus();

            var proposalStatisticsDto = new ProposalStatisticsDto
            {
                totalProposals = ProposalCount,
                ProposalsThisMonth = ProposalThisMonth,
                DistributionBasedOnProposalStatus = ProposalDistributionBasedOnStatus
            };

            return proposalStatisticsDto;
        }
    }
}
