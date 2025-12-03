using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Success>>
{

  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly IEmailTemplateService _emailTemplateService;
  private readonly IEmailService _emailService;

  public ResetPasswordCommandHandler(IIdentityService identityService, IUsersRepository usersRepository, IEmailTemplateService emailTemplateService, IEmailService emailService)
  {
    _identityService = identityService;
    _usersRepository = usersRepository;
    _emailTemplateService = emailTemplateService;
    _emailService = emailService;
  }
  public async Task<Result<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
  {
    var tokenDecoded = Uri.UnescapeDataString(request.Token);
    var resetResult = await _identityService.ResetPasswordAsync(request.UserId, tokenDecoded, request.NewPassword);
    if (resetResult.IsError) return resetResult.Errors;
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(request.UserId);


    if (userProfileResult.IsError) return userProfileResult.Errors;


    var emailTemplateResult = _emailTemplateService.GeneratePasswordChangedEmail(userProfileResult.Value.FullName);

    var userResult = await _identityService.GetUserByIdAsync(request.UserId);
    if (userResult.IsError) return userResult.Errors;

    var emailResult = await _emailService.SendAsync(userResult.Value.Email, EmailSubjects.PasswordChanged, emailTemplateResult, cancellationToken);

    return Result.Success;
  }
}