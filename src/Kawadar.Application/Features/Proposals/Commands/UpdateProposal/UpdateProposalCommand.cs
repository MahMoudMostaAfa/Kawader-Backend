using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposal;




public record UpdateProposalCommand(
Guid ProposalId,
string? CoverLetter,
List<QuestionAnswerUpdateDto>? QuestionAnswerUpdateDtos,
List<MilestoneUpdateDto>? MilestoneUpdateDtos,
decimal? Amount,
int? EstimatedDays,
int? HourlyRate,
int? EstimatedHours
) : IRequest<Result<Updated>>;