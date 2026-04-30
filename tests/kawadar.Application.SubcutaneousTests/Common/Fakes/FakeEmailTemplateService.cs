using Kawadar.Application.Common.Interfaces;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeEmailTemplateService : IEmailTemplateService
{
    public string GenerateWelcomeEmail(string userName, string confirmationLink)
        => $"<html>Welcome {userName}! Confirm: {confirmationLink}</html>";

    public string GenerateEmailConfirmationEmail(string userName, string confirmationLink)
        => $"<html>Confirm email for {userName}: {confirmationLink}</html>";

    public string GeneratePasswordResetEmail(string userName, string resetLink)
        => $"<html>Reset password for {userName}: {resetLink}</html>";

    public string GeneratePasswordChangedEmail(string userName)
        => $"<html>Password changed for {userName}</html>";

    public string GenerateGenericEmail(string title, string message, string? buttonText = null, string? buttonLink = null)
        => $"<html><h1>{title}</h1><p>{message}</p></html>";
}
