using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWallet;

public class GetMyWalletQueryHandler : IRequestHandler<GetMyWalletQuery, Result<WalletDto>>
{

  private readonly IMapper _mapper;

  private readonly IUser _user;
  private readonly IWalletRepository _walletRepository;
  private readonly IUsersRepository _usersRepository;

  public GetMyWalletQueryHandler(IMapper mapper, IUser user, IWalletRepository walletRepository
  , IUsersRepository usersRepository)
  {
    _mapper = mapper;
    _user = user;
    _walletRepository = walletRepository;
    _usersRepository = usersRepository;

  }

  public async Task<Result<WalletDto>> Handle(GetMyWalletQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;

    var wallet = walletResult.Value;

    var walletDto = _mapper.Map<WalletDto>(wallet);

    return walletDto;


  }
}