using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposalStatus;


public record UpdateProposalStatusCommand(
  Guid ProposalId,
  JobProposalStatus NewProposalStatus
) : IRequest<Result<Updated>>;