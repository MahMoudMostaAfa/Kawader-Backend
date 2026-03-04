using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestions;

public record UpdateQuestionItemDto(Guid? Id, string Question, bool IsRequired);

public record UpdateJobQuestionsCommand(
  string Slug,
  List<UpdateQuestionItemDto> Questions
) : IRequest<Result<Updated>>;
