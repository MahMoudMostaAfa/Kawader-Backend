using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Jobs.JobQuestions;

public class JobQuestion : AuditableEntity
{
  public string Question { get; private set; } = "";
  public bool IsRequired { get; private set; } = false;
  public int DisplayOrder { get; private set; } = 1;


  private JobQuestion()
  { }

  private JobQuestion(string question, bool isRequired, int displayOrder) : base(Guid.NewGuid())
  {
    Question = question;
    IsRequired = isRequired;
    DisplayOrder = displayOrder;
  }

  public static Result<JobQuestion> Create(string question, bool isRequired = false, int displayOrder = 1)
      => new JobQuestion(question, isRequired, displayOrder);

  public static Result<List<JobQuestion>> CreateList(List<(string question, bool isRequired)> questions)
  {
    var jobQuestions = new List<JobQuestion>();
    int displayOrder = 1;
    foreach (var (question, isRequired) in questions)
    {
      var jobQuestion = new JobQuestion(question, isRequired, displayOrder++);
      jobQuestions.Add(jobQuestion);
    }

    return jobQuestions;
  }



  public void Update(string question, bool isRequired, int displayOrder)
  {
    Question = question;
    IsRequired = isRequired;
    DisplayOrder = displayOrder;
  }
}