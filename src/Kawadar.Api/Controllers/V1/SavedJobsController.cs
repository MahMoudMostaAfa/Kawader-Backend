using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/saved-jobs")]
[Authorize]
public class SavedJobsController : ApiController
{
  [HttpPost("{jobId:guid}")]
  public Task<IActionResult> SaveJob(Guid jobId, CancellationToken cancellationToken)
  {
    return Task.FromResult<IActionResult>(Ok());
  }


}