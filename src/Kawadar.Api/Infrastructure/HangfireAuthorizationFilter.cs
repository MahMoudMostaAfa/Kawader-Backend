using Hangfire.Dashboard;

namespace Kawadar.Api.Infrastructure;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
  public bool Authorize(DashboardContext context)
  {
    var httpContext = context.GetHttpContext();
    var env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

    return !env.IsProduction();
  }
}
