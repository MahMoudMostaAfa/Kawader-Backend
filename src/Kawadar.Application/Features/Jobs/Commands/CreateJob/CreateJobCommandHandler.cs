namespace Kawadar.Application.Features.Jobs.Commands.CreateJob;

using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using MediatR;


public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<Created>>
{
  private readonly IUser _user;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly ISpecilizationRepository _specilizationRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly IJobsRepository _jobsRepository;
  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;

  public CreateJobCommandHandler(IUser user, IIdentityService identityService, IUsersRepository usersRepository, ISpecilizationRepository specilizationRepository, ISkillRepository skillRepository, IJobsRepository jobsRepository, IStorageClient storageClient, IUnitOfWork unitOfWork)
  {
    _user = user;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _specilizationRepository = specilizationRepository;
    _skillRepository = skillRepository;
    _jobsRepository = jobsRepository;
    _storageClient = storageClient;
    _unitOfWork = unitOfWork;
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


    var jobResult = Kawadar.Domain.Jobs.Job.Create(userProfileId, request.SpecilizationId, request.Title, request.Description, request.JobType, request.BudgetRange, request.HourlyRateRange, request.DurationInDays, request.ExperienceLevel, slug, jobQuestions, skills, attachments: JobFiles);

    if (jobResult.IsError) return jobResult.Errors;

    await _jobsRepository.AddAsync(jobResult.Value, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);




    return Result.Created;
  }
}