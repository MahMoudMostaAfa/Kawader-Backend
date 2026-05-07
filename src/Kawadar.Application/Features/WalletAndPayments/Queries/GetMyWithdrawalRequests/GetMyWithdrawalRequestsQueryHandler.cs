using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWithdrawalRequests;

public class GetMyWithdrawalRequestsQueryHandler : IRequestHandler<GetMyWithdrawalRequestsQuery, Result<List<WithdrawalRequestDto>>>
{
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;

  public GetMyWithdrawalRequestsQueryHandler(IMapper mapper, IUser user, IUsersRepository usersRepository,
    IWalletRepository walletRepository, IWithdrawalRequestRepository withdrawalRequestRepository)
  {
    _mapper = mapper;
    _user = user;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
    _withdrawalRequestRepository = withdrawalRequestRepository;
  }

  public async Task<Result<List<WithdrawalRequestDto>>> Handle(GetMyWithdrawalRequestsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;

    var requestsResult = await _withdrawalRequestRepository.GetByWalletIdAsync(wallet.Id, request.Status, cancellationToken);
    if (requestsResult.IsError) return requestsResult.Errors;

    var dtos = requestsResult.Value.Select(r => _mapper.Map<WithdrawalRequestDto>(r)).ToList();
    return dtos;
  }
}
