using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Mail;

using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Microsoft.Extensions.Configuration;

namespace Kawadar.Infrastructure.Services;

public class EmailService : IEmailService
{
  private readonly IConfiguration _configuration;
  private readonly SmtpSettings _settings;

  public EmailService(IConfiguration configuration)
  {
    _configuration = configuration;
    _settings = new SmtpSettings();
    _configuration.GetSection("Smtp").Bind(_settings);
  }

  public async Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
  {
    return await SendInternalAsync(new[] { to }, subject, htmlBody, cancellationToken);
  }

  public async Task<Result<Success>> SendManyAsync(IEnumerable<string> tos, string subject, string htmlBody, CancellationToken cancellationToken = default)
  {
    return await SendInternalAsync(tos, subject, htmlBody, cancellationToken);
  }

  private async Task<Result<Success>> SendInternalAsync(IEnumerable<string> tos, string subject, string htmlBody, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(_settings.Host))
    {
      return Error.Failure("Email.NotConfigured", "SMTP host is not configured.");
    }

    using var client = new SmtpClient(_settings.Host, _settings.Port)
    {
      EnableSsl = _settings.EnableSsl,
      Credentials = new NetworkCredential(_settings.Username, _settings.Password)
    };

    using var message = new MailMessage
    {
      From = new MailAddress(string.IsNullOrWhiteSpace(_settings.From) ? _settings.Username : _settings.From),
      Subject = subject,
      Body = htmlBody,
      IsBodyHtml = true
    };

    foreach (var to in tos.Where(t => !string.IsNullOrWhiteSpace(t)))
    {
      message.To.Add(to);
    }

    try
    {
      // SmtpClient.SendMailAsync does not accept a CancellationToken in some frameworks.
      await client.SendMailAsync(message);
      return Result.Success;
    }
    catch (SmtpException ex)
    {
      return Error.Failure("Email.SendFailed", ex.Message);
    }
    catch (Exception ex)
    {
      return Error.Unexpected("Email.Unexpected", ex.Message);
    }
  }

  private sealed class SmtpSettings
  {
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? From { get; set; }
  }
}
