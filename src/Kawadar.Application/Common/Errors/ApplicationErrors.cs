using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Errors;

public static class ApplicationErrors
{
  public static Error UserIsNotAuthenticated => Error.Unauthorized("User.NotAuthenticated", "The user is not authenticated.");

  public static Error UnauthorizedAccess => Error.Unauthorized("User.Unauthorized", "You do not have permission to perform this action.");

  public static Error FailedToUpdateProfile => Error.Failure("UserProfile.UpdateFailed", "Failed to update the user profile.");

  public static Error FailedToUploadIdentity => Error.Failure("UserProfile.UploadIdentityFailed", "Failed to upload identity documents.");

  public static Error UserAccountNotActivated => Error.Unauthorized("UserAccount.NotActivated", "Your account is not activated. Please activate your account to access this resource.");
}