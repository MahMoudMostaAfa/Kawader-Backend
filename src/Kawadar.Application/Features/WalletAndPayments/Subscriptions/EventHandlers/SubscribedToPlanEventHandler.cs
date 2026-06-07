using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using Kawadar.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.EventHandlers
{
    public class SubscribedToPlanEventHandler(IUnitOfWork unitOfWork, INotificationsRepository notificationsRepository,
        INotificationsHubService notificationsHubService, ISubscriptionsRepository subscriptionsRepository,
        ILogger<SubscribedToPlanEventHandler> logger, IUsersRepository usersRepository, IIdentityService identityService) : INotificationHandler<SubscribedToPlanEvent>
    {
        public async Task Handle(SubscribedToPlanEvent notification, CancellationToken cancellationToken)
        {
            var userSubscriptionResult = await subscriptionsRepository.GetUserSubscriptionById(notification.UserSubscriptionId);

            var userIdentity = await identityService.GetUserByIdAsync(notification.userId);
            if (userIdentity.IsError)
            {
                logger.LogError("Failed to retrieve user identity with ID {IdentityUserId}: {errors}", notification.userId, userIdentity.Errors);
                return;
            }

            var userProfileResult = await usersRepository.GetUserProfileByIdAsync(notification.UserProfileId);

            if (userProfileResult.IsError)
            {
                logger.LogError("Failed to retrieve user profile with ID {UserProfileId}: {errors}", notification.UserProfileId, userProfileResult.Errors);
                return;
            }

            if (userSubscriptionResult.IsError)
            {
                logger.LogError("Failed to retrieve user subscription with ID {senderUserId}: {errors}", notification.UserProfileId, userSubscriptionResult.Errors);
                return;
            }

            var userSubscription = userSubscriptionResult.Value;
            var notificationResult = Notification.Create(notification.UserProfileId, "Subscription payment", "The Subscription fee has been deducted from your wallet balance succesfully"
                , NotificationCategory.Payment, NotificationType.Success, userSubscription.Id, "UserSubscription");

            if (notificationResult.IsError)
            {
                logger.LogError("Failed to create notification for new Subscription: {errors}", notificationResult.Errors);
                return;
            }
            var newNotification = notificationResult.Value;
            await notificationsRepository.AddNotificationAsync(newNotification);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await notificationsHubService.SendNotificationAsync(notification.userId, new ConversastionsAndMessages.DTOs.NotificationDto
            {
                Id = newNotification.Id,
                Body = newNotification.Body,
                Category = newNotification.Category.ToString(),
                IsRead = newNotification.IsRead,
                ReceivedAt = newNotification.CreatedAt,
                Title = newNotification.Title,
                Type = newNotification.Type.ToString(),
                RedirectUrl = newNotification.RedirectUrl
            });
        }
    }
}