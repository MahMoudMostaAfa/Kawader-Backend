using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobQuestions;

namespace Kawadar.Domain.Proposals.QuestionAnswers;

public class ProposalQuestionAnswer : AuditableEntity
{
  public Guid JobProposalId { get; private set; }

  public Guid QuestionId { get; private set; }

  public JobQuestion Question { get; private set; } = null!;

  public string Answer { get; private set; } = string.Empty;

  private ProposalQuestionAnswer() { }

  private ProposalQuestionAnswer(Guid jobProposalId, Guid questionId, string answer) : base(Guid.NewGuid())
  {
    JobProposalId = jobProposalId;
    QuestionId = questionId;
    Answer = answer;
  }

  public static Result<ProposalQuestionAnswer> Create(Guid jobProposalId, Guid questionId, string answer)
  {
    if (Guid.Empty.Equals(jobProposalId))
      return Error.Validation("Job Proposal ID is required.");

    if (Guid.Empty.Equals(questionId))
      return Error.Validation("Question ID is required.");

    if (string.IsNullOrWhiteSpace(answer))
      return Error.Validation("Answer is required.");

    var questionAnswer = new ProposalQuestionAnswer(jobProposalId, questionId, answer);
    return questionAnswer;
  }


  public Result<Updated> Update(string answer)
  {
    if (string.IsNullOrWhiteSpace(answer))
      return Error.Validation("Answer is required.");

    Answer = answer;
    return Result.Updated;
  }


}