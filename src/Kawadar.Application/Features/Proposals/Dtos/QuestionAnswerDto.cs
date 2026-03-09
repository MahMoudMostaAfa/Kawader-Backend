namespace Kawadar.Application.Features.Proposals.Dtos;


public class QuestionAnswerDto
{
  public Guid QuestionId { set; get; }

  public string QuestionAnswer { set; get; } = String.Empty;
}