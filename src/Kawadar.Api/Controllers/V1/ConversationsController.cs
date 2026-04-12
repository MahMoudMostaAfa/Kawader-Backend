using Asp.Versioning;
using Microsoft.AspNetCore.Components;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/conversations")]
public class ConversationsController : ApiController
{



}