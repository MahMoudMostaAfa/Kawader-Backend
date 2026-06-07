using Kawadar.Domain.Common;

namespace Kawadar.Domain.Contracts.Events;

public class RequestCompletionContractEvent : DomainEvent
{
  public Guid ContractId { get; private set; }

  public RequestCompletionContractEvent(Guid contractId)
  {
    ContractId = contractId;
  }
}