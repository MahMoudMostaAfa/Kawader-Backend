using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IContractsRepository
{

  void Add(Contract contract);
  Task<Result<PaginatedList<Contract>>> GetContractsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

  Task<Result<Contract>> GetContractByIdAsync(Guid contractId, CancellationToken cancellationToken = default);
}