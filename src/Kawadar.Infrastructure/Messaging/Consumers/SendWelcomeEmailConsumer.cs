using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Messaging.Consumers;


public class SendWelcomeEmailConsumer : IConsumer<SendWelcomeEmailMessage>
{

  private readonly IEmailService _emailService;
  private readonly IEmailTemplateService _emailTempleteService;

  private readonly ILogger<SendWelcomeEmailConsumer> _logger;

  public SendWelcomeEmailConsumer(IEmailService emailService, IEmailTemplateService emailTemplateService, ILogger<SendWelcomeEmailConsumer> logger)
  {
    _emailService = emailService;
    _emailTempleteService = emailTemplateService;
    _logger = logger;

  }
  public async Task Consume(ConsumeContext<SendWelcomeEmailMessage> context)
  {
    var message = context.Message;
    _logger.LogInformation("Processing Welocome message for user {UserEmail}", message.Email);

    try
    {
      var templeteResult = _emailTempleteService.GenerateEmailConfirmationEmail(message.FullName, "www.google.com");
      await _emailService.SendAsync(message.Email, "Confirmation email", templeteResult);

      // MassTransit auto-ACKs when Consume() completes without exception ✅

    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "failed to send email to {userEmail}", message.Email);

      throw; // MassTransit will NACK and retry based on cfg policy
    }
  }
}