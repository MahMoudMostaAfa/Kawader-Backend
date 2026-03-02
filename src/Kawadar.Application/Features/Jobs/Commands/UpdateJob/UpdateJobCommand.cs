using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJob;

public record UpdateJobCommand(
string Slug,
string? Title,
string? Description,
Guid? SpecilizationId,
JobType? JobType,
BudgetRange? BudgetRange,
HourlyRateRange? HourlyRateRange,
int? DurationInDays,
JobExperienceLevel? ExperienceLevel
) : IRequest<Result<Updated>>;