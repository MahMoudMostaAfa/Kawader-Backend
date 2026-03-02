using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJobAttachment;

public class DeleteJobAttachmentCommandHandler : IRequestHandler<DeleteJobAttachmentCommand, Result<Deleted>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;

  public DeleteJobAttachmentCommandHandler(
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

  public async Task<Result<Deleted>> Handle(DeleteJobAttachmentCommand request, CancellationToken cancellationToken)
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

    // Get the attachment file URL before removing so we can delete from storage
    var attachment = job.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
    if (attachment is null)
      return JobErrors.JobFileNotFound;

    var fileUrl = attachment.File.FileUrl;

    var removeResult = job.RemoveAttachment(request.AttachmentId);
    if (removeResult.IsError) return removeResult.Errors;

    // Delete from blob storage if it's an uploaded file (not an external link)
    if (attachment.File.MimeType != "link")
    {
      await _storageClient.DeleteFileAsync(fileUrl, Containers.JobAttachements);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Deleted;
  }
}
