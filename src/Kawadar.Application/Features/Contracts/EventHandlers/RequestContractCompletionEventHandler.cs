using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Contracts.Events;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.EventHandlers;

public class RequestContractCompletionEventHandler : INotificationHandler<RequestCompletionContractEvent>
{
  private readonly IContractsRepository _contractsRepository;
  private readonly INotificationsRepository _notificationsRepository;
  private readonly INotificationsHubService _notificationsHubService;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUsersRepository _usersRepository;

  public RequestContractCompletionEventHandler(IContractsRepository contractsRepository, INotificationsRepository notificationsRepository, INotificationsHubService notificationsHubService, IUnitOfWork unitOfWork, IUsersRepository usersRepository)
  {
    _contractsRepository = contractsRepository;
    _notificationsRepository = notificationsRepository;
    _notificationsHubService = notificationsHubService;
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;

  }
  public async Task Handle(RequestCompletionContractEvent notification, CancellationToken cancellationToken)
  {
    var contractResult = await _contractsRepository.GetContractByIdAsync(notification.ContractId);
    if (contractResult.IsError) return;
    var contract = contractResult.Value;

    var notifactionMessageResult = Notification.Create(
      contract.ClientId,
         contract.Title,
      $"The freelancer has requested completion for the contract: {contract.Title}. Please review the request and take the necessary actions.",

      NotificationCategory.Contract,
      NotificationType.Info,
      contract.Id,
      "Contracts",
      $"contracts/{contract.Id}");

    var notifiactionMessage = notifactionMessageResult.Value;

    await _notificationsRepository.AddNotificationAsync(notifiactionMessage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var notificationDto = new NotificationDto
    {
      Id = notifiactionMessage.Id,
      Body = notifiactionMessage.Body,
      Title = notifiactionMessage.Title,
      Category = notifiactionMessage.Category.ToString(),
      Type = notifiactionMessage.Type.ToString(),
      IsRead = notifiactionMessage.IsRead,
      ReceivedAt = notifiactionMessage.CreatedAt,
      RedirectUrl = notifiactionMessage.RedirectUrl
    };


    var userProfileResult = await _usersRepository.GetUserProfileByIdAsync(contract.ClientId);
    if (userProfileResult.IsError) return;
    var userProfile = userProfileResult.Value;

    await _notificationsHubService.SendNotificationAsync(userProfile.UserId, notificationDto);
  }
}