using Microsoft.AspNetCore.Authorization;

namespace Kawadar.Api.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{

  public HasPermissionAttribute(string permission)
  {
    Policy = permission;
  }
}