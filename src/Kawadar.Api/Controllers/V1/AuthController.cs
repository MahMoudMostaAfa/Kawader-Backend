
using System.Threading.Tasks;
using Kawadar.Api.Attributes;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Auth.Commands.ChangePassword;
using Kawadar.Application.Features.Auth.Commands.ConfirmEmail;
using Kawadar.Application.Features.Auth.Commands.ForgetPassword;
using Kawadar.Application.Features.Auth.Commands.Login;
using Kawadar.Application.Features.Auth.Commands.Register;
using Kawadar.Application.Features.Auth.Commands.ResendConfirmationEmail;
using Kawadar.Application.Features.Auth.Commands.ResetPassword;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
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
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)
  ]
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
  public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command, CancellationToken cancellationToken)
  {
    var result = await _sender.Send(command, cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "Password reset email sent successfully." }),
      Problem
    );
  }

  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand, CancellationToken cancellationToken)
  {
    var result = await _sender.Send(resetPasswordCommand, cancellationToken);
    return result.Match(
      _ => Ok(new { Message = "Password has been reset successfully." }),
      Problem
    );
  }
  [HttpPut("change-password")]
  [Authorize]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePassword)
  {
    var result = await _sender.Send(changePassword);

    return result.Match(
       _ => Ok(new { Message = "Password has been changed successfully." }),
       Problem
     );
  }



}




