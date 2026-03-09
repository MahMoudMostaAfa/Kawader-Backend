using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.CreateProposal;

public record CreateProposalCommand(Guid JobId, string CoverLetter, JobProposalType JobProposalType, decimal? Amount, int? EstimatedDays, int? HourlyRate, int? EstimatedHours, List<QuestionAnswerDto>? QuestionAnswerDtos, List<MilestoneDto>? MilestoneDtos) : IRequest<Result<Created>>;