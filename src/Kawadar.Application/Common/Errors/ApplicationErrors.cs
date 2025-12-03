using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Errors;

public static class ApplicationErrors
{
  public static Error UserIsNotAuthenticated => Error.Unauthorized("User.NotAuthenticated", "The user is not authenticated.");
}