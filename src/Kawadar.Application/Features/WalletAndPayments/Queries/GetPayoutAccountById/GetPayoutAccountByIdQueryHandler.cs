using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetPayoutAccountById;

public class GetPayoutAccountByIdQueryHandler : IRequestHandler<GetPayoutAccountByIdQuery, Result<UserPayoutAccountDto>>
{
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;

  public GetPayoutAccountByIdQueryHandler(IMapper mapper, IUser user,
    IUsersRepository usersRepository, IUserPayoutAccountRepository payoutAccountRepository)
  {
    _mapper = mapper;
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
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

    var accountDto = _mapper.Map<UserPayoutAccountDto>(account);

    return accountDto;
  }
}
