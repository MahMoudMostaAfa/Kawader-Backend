using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class EscrowReleaseJob
{

  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<EscrowReleaseJob> _logger;
  private readonly IWalletRepository _walletRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly INotificationsRepository _notificationsRepository;
  private readonly INotificationsHubService _notificationsHubService;
  public EscrowReleaseJob(IUnitOfWork unitOfWork, ILogger<EscrowReleaseJob> logger, IWalletRepository walletRepository, IUsersRepository usersRepository, INotificationsRepository notificationsRepository, INotificationsHubService notificationsHubService)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
    _walletRepository = walletRepository;
    _usersRepository = usersRepository;
    _notificationsRepository = notificationsRepository;
    _notificationsHubService = notificationsHubService;

  }
  public async Task ExecuteAsync(Guid escrowTransactionId, CancellationToken cancellationToken)
  {

    var escrowTransactionResult = await _walletRepository.GetEscrowTransactionById(escrowTransactionId, cancellationToken);
    if (escrowTransactionResult.IsError)
    {
      _logger.LogError("Failed to retrieve escrow transaction with id {EscrowTransactionId}.", escrowTransactionId);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to retrieve escrow transaction with id {escrowTransactionId}.");
    }
    var transaction = escrowTransactionResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(transaction.ReceiverUserId);
    if (walletResult.IsError)
    {
      _logger.LogError("Failed to retrieve wallet for user {UserId}.", transaction.ReceiverUserId);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to retrieve wallet for user {transaction.ReceiverUserId}.");
    }
    var wallet = walletResult.Value;
    wallet.AddTransaction(transaction.Amount, TransactionType
    .EscrowRelease, WalletTransactionReferenceType.Contract, transaction.Id, null, WalletTransactionStatus.Completed);
    await _unitOfWork.SaveChangesAsync(cancellationToken);


    // Send notification to freelancer about released milestone payment

    var freelancerProfileResult = await _usersRepository.GetUserProfileByIdAsync(transaction.ReceiverUserId);
    if (
      freelancerProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve user profile for user {UserId}.", transaction.ReceiverUserId);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to retrieve user profile for user {transaction.ReceiverUserId}.");
    }
    ;

    var freelancerProfile = freelancerProfileResult.Value;

    var notificationResult = Notification.Create(freelancerProfile.Id, "Payment Released", "Your payment for the completed milestone or contract has been released.", Domain.Notifications.Enums.NotificationCategory.Payment, Domain.Notifications.Enums.NotificationType.Success, null, null, null);
    if (notificationResult.IsError)
    {
      _logger.LogError("Failed to create notification for user {UserId}. Errors: {Errors}", freelancerProfile.Id, notificationResult.Errors);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to create notification for user {freelancerProfile.Id}. Errors: {string.Join(", ", notificationResult.Errors.Select(e => e.Description))}");
    }
    var notification = notificationResult.Value;

    await _notificationsRepository.AddNotificationAsync(notification);

    var notificationDto = new NotificationDto
    {
      Body = notification.Body,
      Category = notification.Category.ToString(),
      Type = notification.Type.ToString(),
      Id = notification.Id,
      IsRead = notification.IsRead,
      ReceivedAt = notification.CreatedAt,
      RedirectUrl = notification.RedirectUrl
    ,
      Title = notification.Title
    };

    await _notificationsHubService.SendNotificationAsync(freelancerProfile.UserId, notificationDto);

    await _unitOfWork.SaveChangesAsync(cancellationToken);



  }
}
