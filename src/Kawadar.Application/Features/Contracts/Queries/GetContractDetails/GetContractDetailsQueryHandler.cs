using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Queries.GetContractDetails;


public class GetContractDetailsQueryHandler : IRequestHandler<GetContractDetailsQuery, Result<ContractDetailsDto>>
{
  private readonly IContractsRepository _contractsRepository;
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _IdentityService;

  public GetContractDetailsQueryHandler(IContractsRepository contractsRepository, IMapper mapper, IUser user, IUsersRepository usersRepository, IIdentityService identityService)
  {
    _contractsRepository = contractsRepository;
    _mapper = mapper;
    _user = user;
    _usersRepository = usersRepository;
    _IdentityService = identityService;

  }
  public async Task<Result<ContractDetailsDto>> Handle(GetContractDetailsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.ClientId != userProfile.Id && contract.FreelancerId != userProfile.Id)
    {
      return ApplicationErrors.UnauthorizedAccess;
    }
        

    var OtherPartyId = contract.ClientId == userProfile.Id ? contract.FreelancerId : contract.ClientId;
    var otherPartyProfileResult = await _usersRepository.GetUserProfileByIdAsync(OtherPartyId);
    if (otherPartyProfileResult.IsError) return otherPartyProfileResult.Errors;
    var otherPartyProfile = otherPartyProfileResult.Value;
    var otherPartyUserResult = await _IdentityService.GetUserByIdAsync(otherPartyProfile.UserId);
    if (otherPartyUserResult.IsError) return otherPartyUserResult.Errors;
    var otherPartyUser = otherPartyUserResult.Value;

    var contractDetailsDto = _mapper.Map<ContractDetailsDto>((contract, otherPartyUser, otherPartyProfile));



    return contractDetailsDto;



  }
}