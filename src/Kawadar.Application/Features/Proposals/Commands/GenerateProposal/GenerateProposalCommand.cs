using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.GenerateProposal;

public record GenerateProposalCommand(Guid JobId) : IRequest<Result<string>>;