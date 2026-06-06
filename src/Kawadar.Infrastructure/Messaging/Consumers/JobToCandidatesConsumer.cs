using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging.Messages;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Messaging.Consumers;

public class JobToCandidatesConsumer : IConsumer<JobToCandidatesMessage>
{
  private readonly ILogger<JobToCandidatesConsumer> _logger;
  private readonly IFreelancerVectorStore _vectorStore;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IJobsRepository _jobsRepository;
  private readonly INotificationsRepository _notificationsRepository;
  private readonly INotificationsHubService _notificationsHubService;
  public JobToCandidatesConsumer(ILogger<JobToCandidatesConsumer> logger, IFreelancerVectorStore vectorStore, IUnitOfWork unitOfWork, IJobsRepository jobsRepository
  , INotificationsRepository notificationsRepository, INotificationsHubService notificationsHubService)
  {
    _logger = logger;
    _vectorStore = vectorStore;
    _unitOfWork = unitOfWork;
    _jobsRepository = jobsRepository;
    _notificationsRepository = notificationsRepository;
    _notificationsHubService = notificationsHubService;
  }
  public async Task Consume(ConsumeContext<JobToCandidatesMessage> context)
  {

    _logger.LogInformation("Received JobToCandidatesMessage message: {Message}", context.Message);
    var jobId = context.Message.JobId;
    var jobResult = await _jobsRepository.GetJobByIdAsync(jobId);
    if (jobResult.IsError)
    {
      _logger.LogError("Failed to retrieve job with ID {JobId}: {Errors}", jobId, jobResult.Errors);
      return;
    }

    var job = jobResult.Value;
    var query = job.Title + " " + job.Description + " " + string.Join(" ", job.Skills.Select(s => s.Name));
    var freelancersResult = await _vectorStore.SearchFreelancersIdsAsync(query, 10);
    if (freelancersResult.IsError)
    {
      _logger.LogError("Failed to search freelancers for job ID {JobId}: {Errors}", jobId, freelancersResult.Errors);
      return;
    }
    var freelancers = freelancersResult.Value.Where(f => f.Id != job.PostedById).ToList();
    // Create notifications for the top freelancers

    foreach (var freelancer in freelancers)
    {
      var notifiactionResult = Notification.Create(
       freelancer.Id,
        "New job Recommendation",
        $"New job matching your skills: {job.Title}",
        NotificationCategory.JobRecommendation,
        NotificationType.Info,
        jobId,
        "jobs",
       "/jobs/" + jobId
      );
      if (notifiactionResult.IsError)
      {
        _logger.LogError("Failed to create notification for freelancer ID {FreelancerId} and job ID {JobId}: {Errors}", freelancer.Id, jobId, notifiactionResult.Errors);
        continue;
      }

      var notification = notifiactionResult.Value;
      await _notificationsRepository.AddNotificationAsync(notification);
      await _unitOfWork.SaveChangesAsync();
      await _notificationsHubService.SendNotificationAsync(freelancer.UserId, new NotificationDto
      {
        Id = notification.Id,
        Title = notification.Title,
        Body = notification.Body,
        Category = notification.Category.ToString(),
        Type = notification.Type.ToString(),
        IsRead = notification.IsRead,
        ReceivedAt = notification.CreatedAt,
        RedirectUrl = notification.RedirectUrl
      });
    }





  }
}