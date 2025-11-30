
using Kawadar.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]

public class AuthController : ApiController
{

  private readonly ISender _sender;
  public AuthController(ISender sender)
  {
    _sender = sender;
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
 
}


public class Request
{
  public string Name { get; set; } = "";
}
