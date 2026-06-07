using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;
using Kawadar.Domain.Contracts.Events;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.RequestContractCompletion;

public class RequestContractCompletionCommandHandler : IRequestHandler<RequesContractCompletionCommand, Result<Created>>
{
  private readonly IContractsRepository _contractRepository;
  private readonly IUnitOfWork _unitOfWork;

  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  private readonly INotificationsHubService _notificationsHubService;


  public RequestContractCompletionCommandHandler(IContractsRepository contractsRepository, IUnitOfWork unitOfWork, IUsersRepository usersRepository, IUser user, INotificationsHubService notificationsHubService)
  {
    _contractRepository = contractsRepository;
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;
    _user = user;
    _notificationsHubService = notificationsHubService;

  }
  public async Task<Result<Created>> Handle(RequesContractCompletionCommand request, CancellationToken cancellationToken)
  {

    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractResult = await _contractRepository.GetContractByIdAsync(request.ContractId);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.FreelancerId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    var requestCompletionResult = contract.RequestCompletion();
    if (requestCompletionResult.IsError) return requestCompletionResult.Errors;


    contract.AddDomainEvent(new RequestCompletionContractEvent(contract.Id));

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Created;
  }
}