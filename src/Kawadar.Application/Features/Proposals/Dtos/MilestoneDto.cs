namespace Kawadar.Application.Features.Proposals.Dtos;


public class MilestoneDto
{
  public string Title { set; get; } = String.Empty;

  public string Description { set; get; } = string.Empty;

  public decimal Amount { set; get; }

  public DateTime DueDate { set; get; }

}