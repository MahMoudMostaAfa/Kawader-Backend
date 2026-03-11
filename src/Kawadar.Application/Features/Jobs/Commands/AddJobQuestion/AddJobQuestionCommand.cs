using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobQuestion;

public record AddJobQuestionCommand(
  string Slug,
  string Question,
  bool IsRequired
) : IRequest<Result<Created>>;
