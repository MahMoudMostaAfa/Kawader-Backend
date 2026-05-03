using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetPayoutAccountById;

public class GetPayoutAccountByIdQueryHandler : IRequestHandler<GetPayoutAccountByIdQuery, Result<UserPayoutAccountDto>>
{
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly ILogger<GetPayoutAccountByIdQueryHandler> _logger;

  public GetPayoutAccountByIdQueryHandler(IMapper mapper, IUser user,
    IUsersRepository usersRepository, IUserPayoutAccountRepository payoutAccountRepository, ILogger<GetPayoutAccountByIdQueryHandler> logger)
  {
    _mapper = mapper;
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _logger = logger;
  }

  public async Task<Result<UserPayoutAccountDto>> Handle(GetPayoutAccountByIdQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var accountResult = await _payoutAccountRepository.GetByIdAsync(request.AccountId, cancellationToken);
    if (accountResult.IsError) return accountResult.Errors;
    var account = accountResult.Value;

    // Ensure the account belongs to the authenticated user
    if (account.UserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    _logger.LogInformation("Fetched payout account  details json {accountDetails}", account.AccountDetailsJson);
    var accountDto = _mapper.Map<UserPayoutAccountDto>(account);

    return accountDto;
  }
}
