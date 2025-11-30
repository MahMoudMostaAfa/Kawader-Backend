namespace Kawadar.Api.Services;

using System.Security.Claims;
using Kawadar.Application.Common.Interfaces;

public class CurrentUser : IUser
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUser(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;

  }
  public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}