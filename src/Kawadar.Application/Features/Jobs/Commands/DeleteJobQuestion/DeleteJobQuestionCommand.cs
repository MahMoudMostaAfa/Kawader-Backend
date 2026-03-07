using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJobQuestion;

public record DeleteJobQuestionCommand(
  string Slug,
  Guid QuestionId
) : IRequest<Result<Deleted>>;
