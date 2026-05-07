using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IUserPayoutAccountRepository
{
  void Add(UserPayoutAccount account);

  Task<Result<UserPayoutAccount>> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);

  Task<Result<List<UserPayoutAccount>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Result<UserPayoutAccount>> GetDefaultByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Result<List<UserPayoutAccount>>> GetAllDefaultsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
