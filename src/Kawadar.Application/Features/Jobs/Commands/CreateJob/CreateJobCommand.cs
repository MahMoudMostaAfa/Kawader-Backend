using Kawadar.Application.Features.Jobs.Commands.CreateJob.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Job.Commands.CreateJob;

public record CreateJobCommand(
string Title,
string Description,
Guid SpecilizationId,
JobType JobType,
BudgetRange BudgetRange,
HourlyRateRange HourlyRateRange,
int DurationInDays,
JobExperienceLevel ExperienceLevel,
List<CreateQuestionDto> QuestionDtos,
List<Guid> SkillIds,
List<IFormFile>? AttachmentFiles,
List<string>? AttachmentLinks

) : IRequest<Result<Created>>;