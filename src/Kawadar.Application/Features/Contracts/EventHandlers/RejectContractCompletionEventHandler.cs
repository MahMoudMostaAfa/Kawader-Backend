using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Contracts.Events;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Contracts.EventHandlers;

public class RejectContractCompletionEventHandler : INotificationHandler<RejectCompletionContractEvent>
{

  private readonly INotificationsRepository _notificationsRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly INotificationsHubService _notificationsHubService;
  private readonly ILogger<RejectContractCompletionEventHandler> _logger;
  public RejectContractCompletionEventHandler(INotificationsRepository notificationsRepository, IUnitOfWork unitOfWork, INotificationsHubService notificationsHubService, ILogger<RejectContractCompletionEventHandler> logger)
  {
    _notificationsRepository = notificationsRepository;
    _unitOfWork = unitOfWork;
    _notificationsHubService = notificationsHubService;
    _logger = logger;

  }
  public async Task Handle(RejectCompletionContractEvent notification, CancellationToken cancellationToken)
  {
    var notificationResult = Notification.Create(
      userId: notification.UserProfileId,
      title: "Contract Completion Rejected",
      body: $"Your contract with id {notification.ContractId} has been rejected for completion. Reason: {notification.Reason}",
      NotificationCategory.Contract,
      NotificationType.Info,
      notification.ContractId,
      "Contracts"
      , $"/contracts/{notification.ContractId}"
    );
    if (notificationResult.IsError)
    {
      // log the error
      _logger.LogError("Failed to create notification for RejectContractCompletionEvent. Errors: {Errors}", notificationResult.Errors);
      return;
    }
    var notificationEntity = notificationResult.Value;
    await _notificationsRepository.AddNotificationAsync(notificationEntity);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var notificationDto = new NotificationDto
    {
      Id = notificationEntity.Id,
      Title = notificationEntity.Title,
      Body = notificationEntity.Body,
      Category = notificationEntity.Category.ToString(),
      Type = notificationEntity.Type.ToString(),
      IsRead = notificationEntity.IsRead,
      ReceivedAt = notificationEntity.CreatedAt,
      RedirectUrl = notificationEntity.RedirectUrl
    };


    await _notificationsHubService.SendNotificationAsync(notification.UserId, notificationDto);
  }
}