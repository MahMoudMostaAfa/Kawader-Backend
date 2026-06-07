using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyPayoutAccounts;

public class GetMyPayoutAccountsQueryHandler : IRequestHandler<GetMyPayoutAccountsQuery, Result<List<UserPayoutAccountDto>>>
{
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;


  public GetMyPayoutAccountsQueryHandler(IMapper mapper, IUser user,
    IUsersRepository usersRepository, IUserPayoutAccountRepository payoutAccountRepository)
  {
    _mapper = mapper;
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;

  }

  public async Task<Result<List<UserPayoutAccountDto>>> Handle(GetMyPayoutAccountsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var accountsResult = await _payoutAccountRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (accountsResult.IsError) return accountsResult.Errors;

    var accounts = accountsResult.Value;
    var accountDtos = accounts.Select(account => _mapper.Map<UserPayoutAccountDto>(account)).ToList();
    return accountDtos;
  }
}
