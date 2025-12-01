namespace Kawadar.Application.Common.Interfaces;

public interface IEmailTemplateService
{
  string GenerateWelcomeEmail(string userName, string confirmationLink);
  string GenerateEmailConfirmationEmail(string userName, string confirmationLink);
  string GeneratePasswordResetEmail(string userName, string resetLink);
  string GeneratePasswordChangedEmail(string userName);
  string GenerateGenericEmail(string title, string message, string? buttonText = null, string? buttonLink = null);
}
