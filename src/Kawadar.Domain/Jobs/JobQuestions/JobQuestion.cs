using Kawadar.Domain.Common;

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

  public static JobQuestion Create(string question, bool isRequired = false)
      => new(question, isRequired, 1);

  public void Update(string question, bool isRequired, int displayOrder)
  {
    Question = question;
    IsRequired = isRequired;
    DisplayOrder = displayOrder;
  }
}