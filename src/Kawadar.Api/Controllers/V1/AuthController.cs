

using Kawadar.Application.Features.Auth.Commands.ChangePassword;
using Kawadar.Application.Features.Auth.Commands.ConfirmEmail;
using Kawadar.Application.Features.Auth.Commands.DeleteAccount;
using Kawadar.Application.Features.Auth.Commands.ForgetPassword;
using Kawadar.Application.Features.Auth.Commands.Login;
using Kawadar.Application.Features.Auth.Commands.Register;
using Kawadar.Application.Features.Auth.Commands.ResendConfirmationEmail;
using Kawadar.Application.Features.Auth.Commands.ResetPassword;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]

public class AuthController : ApiController
{

  private readonly ISender _sender;
  private readonly IConfiguration _configuration;


  public AuthController(ISender sender, IConfiguration configuration)
  {
    _sender = sender;
    _configuration = configuration;

  }

  [HttpPost("Register")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(Register))]
  [EndpointSummary("Registers a new user.")]
  [EndpointDescription("Creates a new user account with the provided registration details.")]
  public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
  {
    var result = await _sender.Send(command, cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "User registered successfully." }),
      Problem
    );
  }


  // login
  [HttpPost("Login")]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(Login))]
  [EndpointSummary("Logs in a user and returns a JWT token.")]
  [EndpointDescription("Authenticates a user with the provided credentials and returns a JWT token for authorized access to protected resources.")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
  {
    var result = await _sender.Send(command, ct);
    return result.Match(
      token => Ok(new { Token = token }),
      Problem
    );
  }

  // confirm email
  [HttpGet("confirm-email")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(ConfirmEmail))]
  [EndpointSummary("Confirms a user's email address.")]
  [EndpointDescription("Confirms a user's email address using the provided user ID and confirmation token.")]
  public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
  {
    var Command = new ConfirmEmailCommand(userId, token);

    var result = await _sender.Send(Command);
    return result.Match(
      _ => Redirect($"{_configuration["FrontUrl"]}/email-confirmed"),
      _ => Redirect($"{_configuration["FrontUrl"]}/email-confirmation-failed")
    );
  }


  // resend confirmation email
  [HttpPost("resend-confirmation-email")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(ResendConfirmationEmail))]
  [EndpointSummary("Resends the confirmation email to a user.")]
  [EndpointDescription("Sends a new confirmation email to the user with the provided user ID and token.")]
  public async Task<IActionResult> ResendConfirmationEmail([FromQuery] ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
  {

    var result = await _sender.Send(command, cancellationToken);

    return result.Match(
      _ => Ok(new { Message = "Confirmation email resent successfully." }),
      Problem
    );
  }

  // resest password  

  [HttpPost("forget-password")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(ForgetPassword))]
  [EndpointSummary("Sends a password reset email to the user.")]
  public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command, CancellationToken cancellationToken)
  {
    var result = await _sender.Send(command, cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "Password reset email sent successfully." }),
      Problem
    );
  }

  [HttpPost("reset-password")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(ResetPassword))]
  [EndpointSummary("Resets the user's password.")]
  [EndpointDescription("Resets the user's password using the provided user ID, reset token, and new password.")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand, CancellationToken cancellationToken)
  {
    var result = await _sender.Send(resetPasswordCommand, cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "Password has been reset successfully." }),
      Problem
    );
  }
  [Authorize]
  [HttpPut("change-password")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(ChangePassword))]
  [EndpointSummary("Changes the user's password.")]
  [EndpointDescription("Changes the user's password using the provided current password and new password.")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePassword)
  {
    var result = await _sender.Send(changePassword);

    return result.Match(
       _ => Ok(new { Message = "Password has been changed successfully." }),
       Problem
     );
  }

  [Authorize]
  [HttpDelete("account")]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName(nameof(DeleteAccount))]
  [EndpointSummary("Soft-deletes the authenticated user's account.")]
  [EndpointDescription("Marks the user's account for deletion and schedules permanent deletion after 30 days. Logging in within that period cancels the scheduled deletion.")]
  public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
  {
    var result = await _sender.Send(new DeleteAccountCommand(), cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "Your account has been marked for deletion. It will be permanently deleted after 30 days. Log in again to cancel." }),
      Problem
    );
  }



}




