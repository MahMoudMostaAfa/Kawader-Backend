using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Kawadar.Application.Features.Auth.Commands.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand, Result<Success>>
{

  private readonly IIdentityService _identityService;
  private readonly IEmailService _emailService;
  private readonly IEmailTemplateService _emailTemplateService;
  private readonly IConfiguration _configuration;
  private readonly IUsersRepository _usersRepository;

  public ResendConfirmationEmailCommandHandler(IIdentityService identityService, IEmailService emailService, IEmailTemplateService emailTemplateService, IConfiguration configuration, IUsersRepository usersRepository)
  {
    _identityService = identityService;
    _emailService = emailService;
    _emailTemplateService = emailTemplateService;
    _configuration = configuration;
    _usersRepository = usersRepository;
  }
  public async Task<Result<Success>> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
  {
    var userResult = await _identityService.GetUserByEmailAsync(request.Email);

    if (userResult.IsError) return userResult.Errors;
    if (userResult.Value.EmailConfirmed) return Error.Validation("User.EmailConfirmed", "Email is already confirmed.");

    var result = await _identityService.GenerateEmailConfirmationTokenAsync(userResult.Value.Id);
    if (result.IsError) return result.Errors;

    var linkURL = $"{_configuration["BackUrl"]}/api/v1/auth/confirm-email?userId={userResult.Value.Id}&token={Uri.EscapeDataString(result.Value)}";

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userResult.Value.Id);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var emailTemplateResult = _emailTemplateService.GenerateEmailConfirmationEmail(userProfileResult.Value.FullName, linkURL);

    await _emailService.SendAsync(request.Email, EmailSubjects.ConfirmEmail, emailTemplateResult, cancellationToken);


    return Result.Success;
  }
}