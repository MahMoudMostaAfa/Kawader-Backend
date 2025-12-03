using AutoMapper;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.UserProfiles.Events;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Kawadar.Application.Features.Auth.EventHandlers;

public class CreatedUserEventHandler : INotificationHandler<CreatedUserEvent>
{

  private readonly IIdentityService _identityService;
  private readonly IEmailService _emailService;
  private readonly IEmailTemplateService _emailTemplateService;
  private readonly IConfiguration _configuration;
  public CreatedUserEventHandler(IIdentityService identityService, IEmailService emailService, IEmailTemplateService emailTemplateService, IConfiguration configuration)
  {
    _identityService = identityService;
    _emailService = emailService;
    _emailTemplateService = emailTemplateService;
    _configuration = configuration;
  }
  public async Task Handle(CreatedUserEvent notification, CancellationToken cancellationToken)
  {
    var tokenResult = await _identityService.GenerateEmailConfirmationTokenAsync(notification.UserId);
    if (tokenResult.IsError) return;
    var linkUrl = $"{_configuration["BackUrl"]}/api/v1/auth/confirm-email?userId={notification.UserId}&token={Uri.EscapeDataString(tokenResult.Value)}";

    var emailTemplateResult = _emailTemplateService.GenerateWelcomeEmail(notification.FirstName, linkUrl);

    await _emailService.SendAsync(notification.Email, "Welcome to Kawadar", emailTemplateResult, cancellationToken);

  }
}