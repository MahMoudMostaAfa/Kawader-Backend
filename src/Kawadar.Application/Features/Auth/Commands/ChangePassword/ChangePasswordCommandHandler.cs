using System.Security.Principal;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<Updated>>
{

  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly IEmailService _emailService;
  private readonly IEmailTemplateService _emailTemplateService;
  private readonly IUser _user;

  public ChangePasswordCommandHandler(IIdentityService identityService, IUsersRepository usersRepository, IEmailService emailService, IEmailTemplateService emailTemplateService, IUser user)
  {
    _identityService = identityService;
    _usersRepository = usersRepository;
    _emailService = emailService;
    _emailTemplateService = emailTemplateService;
    _user = user;
  }
  public async Task<Result<Updated>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var identityResult = await _identityService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
    if (identityResult.IsError) return identityResult.Errors;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    var emailTempleteResult = _emailTemplateService.GeneratePasswordChangedEmail(userProfileResult.Value.FullName);

    var userResult = await _identityService.GetUserByIdAsync(userId);

    var emailResult = await _emailService.SendAsync(userResult.Value.Email, EmailSubjects.PasswordChanged, emailTempleteResult);

    if (emailResult.IsError) return emailResult.Errors;

    return Result.Updated;
  }
}