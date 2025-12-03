using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Kawadar.Application.Features.Auth.Commands.ForgetPassword;

public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result<Success>>
{

  private readonly IIdentityService _identityService;
  private readonly IConfiguration _configuration;

  private readonly IEmailTemplateService _emailTemplateService;
  private readonly IEmailService _emailService;
  private readonly IUsersRepository _usersRepository;
  public ForgetPasswordCommandHandler(IIdentityService identityService, IConfiguration configuration, IEmailTemplateService emailTemplateService, IEmailService emailService, IUsersRepository usersRepository)
  {
    _identityService = identityService;
    _configuration = configuration;
    _emailTemplateService = emailTemplateService;
    _emailService = emailService;
    _usersRepository = usersRepository;


  }
  public async Task<Result<Success>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
  {
    var userResult = await _identityService.GetUserByEmailAsync(request.Email);

    if (userResult.IsError) return Result.Success; // to prevent email enumeration attacks

    var tokenResult = await _identityService.GeneratePasswordResetTokenAsync(userResult.Value.Id);
    if (tokenResult.IsError) return tokenResult.Errors;
    var token = Uri.EscapeDataString(tokenResult.Value);

    var LinkUrl = $"{_configuration["FrontUrl"]}/reset-password?userId={userResult.Value.Id}&token={token}";

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userResult.Value.Id);
    if (userProfileResult.IsError) return userProfileResult.Errors;


    var emailTemplateResult = _emailTemplateService.GeneratePasswordResetEmail(userProfileResult.Value.FullName, LinkUrl);


    var sendEmailResult = await _emailService.SendAsync(request.Email, EmailSubjects.ResetPassword, emailTemplateResult, cancellationToken);
    if (sendEmailResult.IsError) return sendEmailResult.Errors;
    return Result.Success;
  }
}