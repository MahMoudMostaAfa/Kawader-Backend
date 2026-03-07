namespace Kawadar.Api.Requests.Job;

public class AddJobQuestionRequest
{
  public string Question { get; set; } = "";
  public bool IsRequired { get; set; }
}
