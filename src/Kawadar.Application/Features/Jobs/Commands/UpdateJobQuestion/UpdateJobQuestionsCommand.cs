using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestion;

public record UpdateJobQuestionCommand(
  string Slug,
  Guid QuestionId,
  string Question,
  bool IsRequired
) : IRequest<Result<Updated>>;
