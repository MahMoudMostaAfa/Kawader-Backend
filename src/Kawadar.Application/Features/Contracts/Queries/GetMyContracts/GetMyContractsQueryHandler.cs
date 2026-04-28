using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Queries.GetMyContracts;

public class GetMyContractsQueryHandler : IRequestHandler<GetMyContractsQuery, Result<PaginatedList<ContractDto>>>
{
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;

  private readonly IMapper _mapper;
  private readonly IUser _user;

  public GetMyContractsQueryHandler(IUsersRepository usersRepository, IContractsRepository contractsRepository, IMapper mapper, IUser user)
  {
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
    _mapper = mapper;
    _user = user;


  }
  public async Task<Result<PaginatedList<ContractDto>>> Handle(GetMyContractsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractsResult = await _contractsRepository.GetContractsByUserIdAsync(userProfile.Id, request.PageNumber, request.PageSize, cancellationToken);

    var contracts = contractsResult.Value.Items;

    var contractsDto = contracts.Select(c => _mapper.Map<ContractDto>((c, userProfile.Id))).ToList();

    var paginatedList = new PaginatedList<ContractDto>(contractsDto, contractsResult.Value.TotalCount, request.PageNumber, request.PageSize);
    return paginatedList;
  }
}