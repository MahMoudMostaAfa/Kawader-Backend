using Kawadar.Domain.Common;

namespace Kawadar.Domain.Contracts.Events;



public class RejectCompletionContractEvent : DomainEvent
{
  public Guid ContractId { get; private set; }

  public Guid UserProfileId { get; private set; }
  public string UserId { get; private set; } = string.Empty;
  public string Reason
  { get; private set; } = string.Empty;

  public RejectCompletionContractEvent(Guid contractId, Guid userProfileId, string userId, string reason)
  {
    ContractId = contractId;
    UserProfileId = userProfileId;
    UserId = userId;
    Reason = reason;

  }
}