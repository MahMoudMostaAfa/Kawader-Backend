using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobFiles;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobAttachment;

public class AddJobAttachmentCommandHandler : IRequestHandler<AddJobAttachmentCommand, Result<Created>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;

  public AddJobAttachmentCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    IUsersRepository usersRepository,
    IStorageClient storageClient,
    IUnitOfWork unitOfWork)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _storageClient = storageClient;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Created>> Handle(AddJobAttachmentCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(request.Slug);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.PostedById != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    Domain.Common.ValueObjects.FileInfo fileInfo;

    if (request.File is not null)
    {
      var fileUrlResult = await _storageClient.UploadFileAsync(
        request.File.OpenReadStream(),
        request.File.FileName,
        Containers.JobAttachements,
        cancellationToken);

      if (fileUrlResult.IsError) return fileUrlResult.Errors;

      fileInfo = new Domain.Common.ValueObjects.FileInfo
      {
        FileName = request.File.FileName,
        FileUrl = fileUrlResult.Value,
        FileSizeInBytes = request.File.Length,
        MimeType = request.File.ContentType
      };
    }
    else
    {
      fileInfo = new Domain.Common.ValueObjects.FileInfo
      {
        FileName = request.ExternalUrl!,
        FileUrl = request.ExternalUrl!,
        MimeType = "link"
      };
    }

    var jobFileResult = JobFile.Create(fileInfo);
    if (jobFileResult.IsError) return jobFileResult.Errors;

    var addResult = job.AddAttachment(jobFileResult.Value);
    if (addResult.IsError) return addResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Created;
  }
}
