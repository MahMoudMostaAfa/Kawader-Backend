using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Api.Requests.Contracts;


public class CreateContractRequest
{
  public Guid JobId { get; set; }
  public Guid ProposaslId { get; set; }
  public ContractType ContractType { get; set; }
  public DateTime StartDate { get; set; }

}