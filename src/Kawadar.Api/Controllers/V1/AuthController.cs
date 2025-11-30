
using System.Threading.Tasks;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]

public class AuthController : ApiController
{

  private readonly ISender _sender;
  private readonly ITokenProvider _tokenProvider;
  public AuthController(ISender sender, ITokenProvider tokenProvider)
  {
    _sender = sender;
    _tokenProvider = tokenProvider;
  }


  [HttpGet]

  public IActionResult Get()
  {
    return Ok("Auth Controller is working!");
  }


  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] Request request)
  {
    var name = request.Name;
    {
      var RegisterCommand = new RegisterCommand(name);
      var result = await _sender.Send(RegisterCommand);

      return Ok(result);
    }
  }
  [HttpGet("token")]
  public async Task<IActionResult> GetToken()
  {
    var token = await _tokenProvider.GenerateTokenAsync("7ebb61a5-a142-47c2-83d6-9b1bd802606d");
    return Ok(new { Token = token.Value });
  }

  [HttpGet("testauth")]

  [Authorize(Policy = Permissions.ApproveUsers)]
  public IActionResult TestAuth()
  {
    return Ok("You are authorized!");
  }

}


public class Request
{
  public string Name { get; set; } = "";
}
