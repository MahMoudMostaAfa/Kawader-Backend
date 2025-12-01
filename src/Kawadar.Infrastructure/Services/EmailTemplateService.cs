using Kawadar.Application.Common.Interfaces;

namespace Kawadar.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
  private const string BaseTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{TITLE}</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            background-color: #f4f4f4;
            padding: 20px;
        }
        .email-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        }
        .email-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 40px 30px;
            text-align: center;
        }
        .email-header h1 {
            color: #ffffff;
            font-size: 28px;
            font-weight: 600;
            margin: 0;
        }
        .email-body {
            padding: 40px 30px;
        }
        .email-body h2 {
            color: #333333;
            font-size: 22px;
            margin-bottom: 20px;
            font-weight: 600;
        }
        .email-body p {
            color: #666666;
            font-size: 16px;
            margin-bottom: 20px;
            line-height: 1.8;
        }
        .button-container {
            text-align: center;
            margin: 35px 0;
        }
        .button {
            display: inline-block;
            padding: 14px 40px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 6px;
            font-weight: 600;
            font-size: 16px;
            transition: transform 0.2s;
        }
        .button:hover {
            transform: translateY(-2px);
        }
        .divider {
            height: 1px;
            background-color: #e0e0e0;
            margin: 30px 0;
        }
        .email-footer {
            background-color: #f8f9fa;
            padding: 30px;
            text-align: center;
            border-top: 1px solid #e0e0e0;
        }
        .email-footer p {
            color: #999999;
            font-size: 14px;
            margin-bottom: 10px;
        }
        .email-footer a {
            color: #667eea;
            text-decoration: none;
        }
        .info-box {
            background-color: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 15px 20px;
            margin: 20px 0;
            border-radius: 4px;
        }
        .info-box p {
            margin: 0;
            color: #666666;
            font-size: 14px;
        }
        @media only screen and (max-width: 600px) {
            .email-body {
                padding: 30px 20px;
            }
            .email-header {
                padding: 30px 20px;
            }
            .email-header h1 {
                font-size: 24px;
            }
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Kawadar</h1>
        </div>
        <div class=""email-body"">
            {CONTENT}
        </div>
        <div class=""email-footer"">
            <p>© 2025 Kawadar. All rights reserved.</p>
            <p>If you have any questions, contact us at <a href=""mailto:support@kawadar.com"">support@kawadar.com</a></p>
        </div>
    </div>
</body>
</html>";

  public string GenerateWelcomeEmail(string userName, string confirmationLink)
  {
    var content = $@"
            <h2>Welcome to Kawadar, {userName}! 🎉</h2>
            <p>Thank you for joining our community. We're excited to have you on board!</p>
            <p>To get started, please verify your email address by clicking the button below:</p>
            <div class=""button-container"">
                <a href=""{confirmationLink}"" class=""button"">Verify Email Address</a>
            </div>
            <div class=""info-box"">
                <p><strong>Note:</strong> This link will expire in 24 hours for security reasons.</p>
            </div>
            <p>If you didn't create an account with us, please ignore this email.</p>";

    return BaseTemplate
        .Replace("{TITLE}", "Welcome to Kawadar")
        .Replace("{CONTENT}", content);
  }

  public string GenerateEmailConfirmationEmail(string userName, string confirmationLink)
  {
    var content = $@"
            <h2>Confirm Your Email Address</h2>
            <p>Hi {userName},</p>
            <p>Please confirm your email address by clicking the button below:</p>
            <div class=""button-container"">
                <a href=""{confirmationLink}"" class=""button"">Confirm Email</a>
            </div>
            <div class=""info-box"">
                <p><strong>Security Note:</strong> This confirmation link will expire in 24 hours.</p>
            </div>
            <div class=""divider""></div>
            <p style=""font-size: 14px; color: #999999;"">If you can't click the button, copy and paste this link into your browser:</p>
            <p style=""font-size: 14px; color: #667eea; word-break: break-all;"">{confirmationLink}</p>";

    return BaseTemplate
        .Replace("{TITLE}", "Confirm Your Email")
        .Replace("{CONTENT}", content);
  }

  public string GeneratePasswordResetEmail(string userName, string resetLink)
  {
    var content = $@"
            <h2>Reset Your Password</h2>
            <p>Hi {userName},</p>
            <p>We received a request to reset your password. Click the button below to create a new password:</p>
            <div class=""button-container"">
                <a href=""{resetLink}"" class=""button"">Reset Password</a>
            </div>
            <div class=""info-box"">
                <p><strong>Important:</strong> This link will expire in 1 hour for security reasons.</p>
            </div>
            <p>If you didn't request a password reset, please ignore this email or contact support if you're concerned about your account security.</p>
            <div class=""divider""></div>
            <p style=""font-size: 14px; color: #999999;"">If you can't click the button, copy and paste this link into your browser:</p>
            <p style=""font-size: 14px; color: #667eea; word-break: break-all;"">{resetLink}</p>";

    return BaseTemplate
        .Replace("{TITLE}", "Reset Your Password")
        .Replace("{CONTENT}", content);
  }

  public string GeneratePasswordChangedEmail(string userName)
  {
    var content = $@"
            <h2>Password Changed Successfully ✓</h2>
            <p>Hi {userName},</p>
            <p>This is a confirmation that your password was successfully changed.</p>
            <div class=""info-box"">
                <p><strong>Security Alert:</strong> If you didn't make this change, please contact our support team immediately.</p>
            </div>
            <p>Your account security is important to us. If you have any concerns, please don't hesitate to reach out.</p>";

    return BaseTemplate
        .Replace("{TITLE}", "Password Changed")
        .Replace("{CONTENT}", content);
  }

  public string GenerateGenericEmail(string title, string message, string? buttonText = null, string? buttonLink = null)
  {
    var buttonHtml = "";
    if (!string.IsNullOrWhiteSpace(buttonText) && !string.IsNullOrWhiteSpace(buttonLink))
    {
      buttonHtml = $@"
            <div class=""button-container"">
                <a href=""{buttonLink}"" class=""button"">{buttonText}</a>
            </div>";
    }

    var content = $@"
            <h2>{title}</h2>
            <p>{message}</p>
            {buttonHtml}";

    return BaseTemplate
        .Replace("{TITLE}", title)
        .Replace("{CONTENT}", content);
  }
}
