using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobSkills;

public record UpdateJobSkillsCommand(
  string Slug,
  List<Guid> SkillIds
) : IRequest<Result<Updated>>;
