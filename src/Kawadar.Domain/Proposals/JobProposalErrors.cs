using Kawadar.Domain.Common.Results;

public static class JobProposalErrors
{
  public static Error JobIDRequired => Error.Validation("jobId", "Job ID is required.");
  public static Error FreelancerIDRequired => Error.Validation("freelancerId", "Freelancer ID is required.");

  public static Error CoverLetterRequired => Error.Validation("coverLetter", "Cover letter is required.");

  public static Error AmountRequiredForOneTime => Error.Validation("amount", "Amount is required for one-time proposals.");

  public static Error EstimatedHoursRequiredForHourly => Error.Validation("estimatedHours", "Estimated hours are required for hourly proposals.");

  public static Error ProposalAlreadyExistsForJob => Error.Conflict("Proposal.Duplicate", "You already submitted a proposal for this job.");
}



