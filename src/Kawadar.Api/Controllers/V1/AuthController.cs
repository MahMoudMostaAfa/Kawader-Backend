
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]

public class AuthController : ApiController
{
  [HttpGet]

  public IActionResult Get()
  {
    return Ok("Auth Controller is working!");
  }
}