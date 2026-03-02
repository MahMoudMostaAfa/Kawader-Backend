namespace Kawadar.Api.Requests.Job;

public class UpdateJobQuestionsRequest
{
  public List<UpdateQuestionItem> Questions { get; set; } = [];
}

public class UpdateQuestionItem
{
  public Guid? Id { get; set; }
  public string Question { get; set; } = "";
  public bool IsRequired { get; set; }
}
