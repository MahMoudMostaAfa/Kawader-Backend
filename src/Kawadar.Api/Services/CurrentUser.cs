namespace Kawadar.Api.Services;

using System.Collections.Generic;
using System.Security.Claims;
using Kawadar.Application.Common.Interfaces.Auth;

public class CurrentUser : IUser
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUser(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;

  }
  public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

  // get Permissions claims
  public List<string> Claims => _httpContextAccessor.HttpContext?.User?.Claims
      .Where(c => c.Type == "Permission")
      .Select(c => c.Value)
      .ToList() ?? new List<string>();

}