using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.UserProfiles;

public static class UserProfileErrors
{
  public static Error UserIdIsRequired => Error.Validation(
    "UserProfile.UserIdIsRequired",
    "User Id is required to create a user profile.");

  public static Error FirstNameIsRequired => Error.Validation(
  "UserProfile.FirstNameIsRequired", "First name is required to create a user profile.");

  public static Error LastNameIsRequired => Error.Validation(
  "UserProfile.LastNameIsRequired", "Last name is required to create a user profile.");

  public static Error FreelancerOrClientOnlyCanRegister => Error.Failure("UserProfile.InvalidProfileType", "Invalid profile type. Only Freelancer and Client profile type is allowed for registration.");


  public static Error ProfilePictureUrlIsRequired => Error.Validation("UserProfile.ProfilePictureUrlIsRequired", "Profile picture URL is required.");

  static public Error IdentityAlreadyVerified => Error.Validation("UserProfile.IdentityAlreadyVerified", "The identity for this user profile has already been verified.");

}