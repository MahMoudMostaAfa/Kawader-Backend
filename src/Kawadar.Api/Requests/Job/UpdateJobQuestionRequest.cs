namespace Kawadar.Api.Requests.Job;

public class UpdateJobQuestionRequest
{
  public string Question { get; set; } = "";
  public bool IsRequired { get; set; }
}
