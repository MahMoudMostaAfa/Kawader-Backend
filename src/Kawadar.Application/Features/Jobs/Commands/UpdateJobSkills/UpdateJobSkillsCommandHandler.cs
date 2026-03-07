using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobSkills;

public class UpdateJobSkillsCommandHandler : IRequestHandler<UpdateJobSkillsCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateJobSkillsCommandHandler(
    IUser user,
    IJobsRepository jobsRepository,
    ISkillRepository skillRepository,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork)
  {
    _user = user;
    _jobsRepository = jobsRepository;
    _skillRepository = skillRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(UpdateJobSkillsCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(Uri.UnescapeDataString(request.Slug));
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.PostedById != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var newSkillsResult = await _skillRepository.GetBySkillIds(request.SkillIds);
    if (newSkillsResult.IsError) return newSkillsResult.Errors;
    var newSkills = newSkillsResult.Value.ToList();

    // Remove skills that are not in the new list
    var existingSkillIds = job.Skills.Select(s => s.Id).ToList();
    var skillIdsToRemove = existingSkillIds.Except(request.SkillIds).ToList();
    foreach (var skillId in skillIdsToRemove)
    {
      var removeResult = job.RemoveSkill(skillId);
      if (removeResult.IsError) return removeResult.Errors;
    }

    // Add skills that are not already in the job
    var skillIdsToAdd = request.SkillIds.Except(existingSkillIds).ToList();
    foreach (var skill in newSkills.Where(s => skillIdsToAdd.Contains(s.Id)))
    {
      var addResult = job.AddSkill(skill);
      if (addResult.IsError) return addResult.Errors;
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
