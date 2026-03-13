using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.DeleteProposal;


public record DeleteProposalCommand(Guid ProposalId) : IRequest<Result<Deleted>>;