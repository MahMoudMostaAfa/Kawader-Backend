namespace Kawadar.Application.Features.Jobs.Commands.CreateJob;

using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.Notifications;
using MediatR;
using Kawadar.Domain.Notifications.Enums;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Jobs.Events;
using Kawadar.Application.Common.Messaging;
using Kawadar.Application.Common.Messaging.Messages;
using Kawadar.Application.Common.Interfaces.Caching;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<Created>>
{
  private readonly IUser _user;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly ISpecilizationRepository _specilizationRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly IJobsRepository _jobsRepository;
  private readonly IStorageClient _storageClient;
  private readonly INotificationsRepository _notificationsRepository;
  private readonly INotificationsHubService _notificationsHubService;

  private readonly IRecommendationService _recommendationService;
  private readonly IEventBus _eventBus;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICacheInvalidator _cacheInvalidator;


  public CreateJobCommandHandler(IUser user, IIdentityService identityService, IUsersRepository usersRepository, ISpecilizationRepository specilizationRepository, ISkillRepository skillRepository, IJobsRepository jobsRepository, IStorageClient storageClient, IUnitOfWork unitOfWork
  , INotificationsRepository notificationsRepository, INotificationsHubService notificationsHubService, IRecommendationService recommendationService
  , IEventBus eventBus, ICacheInvalidator cacheInvalidator)
  {
    _user = user;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _specilizationRepository = specilizationRepository;
    _skillRepository = skillRepository;
    _jobsRepository = jobsRepository;
    _storageClient = storageClient;
    _unitOfWork = unitOfWork;
    _notificationsRepository = notificationsRepository;
    _notificationsHubService = notificationsHubService;
    _recommendationService = recommendationService;
    _eventBus = eventBus;
    _cacheInvalidator = cacheInvalidator;
  }
  public async Task<Result<Created>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;
    var userResult = await _identityService.GetUserByIdAsync(userId);
    if (userResult.IsError) return userResult.Errors;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfileId = userProfileResult.Value.Id;
    var userProfile = userProfileResult.Value;
    if (userProfile.IsBanned || userProfile.IsDeleted) return ApplicationErrors.UnauthorizedAccess;
    if (userProfile.IsActivated == false || userProfile.IsIdentityVerified == false) return ApplicationErrors.UserAccountNotActivated;

    var SpecilizationResult = await _specilizationRepository.GetById(request.SpecilizationId);
    if (SpecilizationResult.IsError) return SpecilizationResult.Errors;

    var skillsResult = await _skillRepository.GetBySkillIds(request.SkillIds);
    if (skillsResult.IsError) return skillsResult.Errors;
    var skills = skillsResult.Value.ToList();


    var JobQuestionsResult = JobQuestion.CreateList(request.QuestionDtos.Select(q => (q.Question, q.IsRequired)).ToList());
    if (JobQuestionsResult.IsError) return JobQuestionsResult.Errors;
    var jobQuestions = JobQuestionsResult.Value;

    var slug = Kawadar.Domain.Jobs.Job.GenerateSlug(request.Title).Value;

    do
    {
      var existingJob = await _jobsRepository.GetJobBySlugAsync(slug);
      if (existingJob.IsSuccess) slug = Kawadar.Domain.Jobs.Job.GenerateSlug(request.Title).Value;
      else break;

    } while (true);

    var JobFiles = new List<JobFile>();

    foreach (var file in request.AttachmentFiles ?? [])
    {
      var fileUrlResult = await _storageClient.UploadFileAsync(file.OpenReadStream(), file.FileName, Containers.JobAttachements, cancellationToken);
      if (fileUrlResult.IsError) return fileUrlResult.Errors;

      var fileInfo = new Kawadar.Domain.Common.ValueObjects.FileInfo()
      {
        FileName = file.FileName,
        FileUrl = fileUrlResult.Value,
        FileSizeInBytes = file.Length,
        MimeType = file.ContentType
      };

      var jobFileResult = JobFile.Create(fileInfo);
      if (jobFileResult.IsError) return jobFileResult.Errors;
      JobFiles.Add(jobFileResult.Value);
    }

    foreach (var link in request.AttachmentLinks ?? [])
    {
      var fileInfo = new Kawadar.Domain.Common.ValueObjects.FileInfo()
      {
        FileName = link,
        FileUrl = link,
        MimeType = "link"
      };

      var jobFileResult = JobFile.Create(fileInfo);
      if (jobFileResult.IsError) return jobFileResult.Errors;
      JobFiles.Add(jobFileResult.Value);
    }


    UserProfile? privateToUserProfile = null;
    // check if is job private and if private to user id is valid
    if (request.IsPrivate && request.PrivateToUserId.HasValue)
    {
      var userProfilePrivateToResult = await _usersRepository.GetUserProfileByIdAsync(request.PrivateToUserId.Value);
      if (userProfilePrivateToResult.IsError) return userProfilePrivateToResult.Errors;
      privateToUserProfile = userProfilePrivateToResult.Value;
    }

    var jobResult = Kawadar.Domain.Jobs.Job.Create(userProfileId, request.SpecilizationId, request.Title, request.Description, request.JobType, request.BudgetRange, request.HourlyRateRange, request.DurationInDays, request.ExperienceLevel, slug, jobQuestions, skills, attachments: JobFiles
    , request.IsPrivate, request.PrivateToUserId
    );

    if (jobResult.IsError) return jobResult.Errors;

    await _jobsRepository.AddAsync(jobResult.Value, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // If the job is private, send a notification to the specified user
    if (request.IsPrivate && privateToUserProfile != null)
    {
      var notificationResult = Notification.Create(privateToUserProfile.Id, "A Job Invitation For You", $"You have been invited to apply for a private job: {request.Title}", NotificationCategory.Job, NotificationType.Success, jobResult.Value.Id, "jobs", $"/jobs/{jobResult.Value.Id}"); ;

      if (notificationResult.IsError) return notificationResult.Errors;

      var notification = notificationResult.Value;
      await _notificationsRepository.AddNotificationAsync(notification, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      // Send real-time notification to the user
      var notifactionDto = new NotificationDto
      {
        Id = notification.Id,
        Title = notification.Title,
        Body = notification.Body,
        Category = notification.Category.ToString(),
        Type = notification.Type.ToString(),
        IsRead = notification.IsRead,
        ReceivedAt = notification.CreatedAt,
        RedirectUrl = notification.RedirectUrl
      };

      await _notificationsHubService.SendNotificationAsync(privateToUserProfile.UserId, notifactionDto);

    }

    var job = jobResult.Value;


    // Add the new job to the recommendation engine as an item
    var labels = job.Skills.Select(s => s.Name.ToLower())
      .Concat(new[] { job.JobType.ToString().ToLower(), job.ExperienceLevel.ToString().ToLower() })
      .ToArray();

    var recommendationItemResult = await _recommendationService.InsertItemAsync(
      job.Id.ToString(),
      categories: new[] { SpecilizationResult.Value.Name },
      labels: labels,
      comment: job.Title,
      ct: cancellationToken);



    if (job.IsPrivate is false)
    {
      // Publish a message to notify candidates about the new job
      await _eventBus.PublishAsync(new JobToCandidatesMessage { JobId = job.Id }, cancellationToken);

    }
    // Invalidate relevant cache entries
    await _cacheInvalidator.EvictByTagAsync(CacheTags.JobsAll, cancellationToken);


    return Result.Created;
  }
}