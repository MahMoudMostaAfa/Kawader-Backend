using Microsoft.AspNetCore.Diagnostics;

namespace Kawadar.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly IProblemDetailsService _problemDetailsService;
  public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
  {
    _problemDetailsService = problemDetailsService;
  }

  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
    {
      HttpContext = httpContext,
      ProblemDetails = new()
      {
        Title = exception.GetType().Name,
        Status = StatusCodes.Status500InternalServerError,
        Detail = exception.Message
      }
    });
  }
}