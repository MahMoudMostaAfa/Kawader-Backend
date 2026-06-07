using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.QuestionAnswers;

namespace Kawadar.Tests.Common.Proposals;

public static class ProposalQuestionAnswerFactory
{
  public static ProposalQuestionAnswerBuilder Builder() => new();

  public static ProposalQuestionAnswer CreateValid() => Builder().Build();
}

public sealed class ProposalQuestionAnswerBuilder
{
  private Guid _jobProposalId = Guid.NewGuid();
  private Guid _questionId = Guid.NewGuid();
  private string _answer = "Valid answer";

  public ProposalQuestionAnswerBuilder WithJobProposalId(Guid value)
  {
    _jobProposalId = value;
    return this;
  }

  public ProposalQuestionAnswerBuilder WithQuestionId(Guid value)
  {
    _questionId = value;
    return this;
  }

  public ProposalQuestionAnswerBuilder WithAnswer(string value)
  {
    _answer = value;
    return this;
  }

  public Result<ProposalQuestionAnswer> BuildResult() =>
    ProposalQuestionAnswer.Create(_jobProposalId, _questionId, _answer);

  public ProposalQuestionAnswer Build()
  {
    var result = BuildResult();
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build ProposalQuestionAnswer: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }
}
