using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Domain.UserProfiles;

public class UserProfile : AuditableEntity
{
  public ProfileType ProfileType { get; private set; } = ProfileType.Freelancer;
  // Personal Information
  public string FirstName { get; private set; } = "";
  public string LastName { get; private set; } = "";

  public string? Title { get; private set; } = "";

  public string? Bio { get; private set; } = "";
  public ExperienceYear ExperienceYear { get; private set; } = ExperienceYear.LessThanOneYear;
  public string? ProfilePictureUrl { get; private set; } = "";
  public string? VideoLink { get; private set; } = "";
  public string? PhoneNumber { get; private set; } = "";
  // availability Status
  public bool IsAvailable { get; private set; } = true;


  public DateTime? DateOfBirth { get; private set; }

  // Ban Status
  public bool IsBanned { get; private set; } = false;
  public DateTime? BannedUntil { get; private set; }

  // Activation Status
  public bool IsActivated { get; private set; } = false;
  public DateTime? ActivatedAt { get; private set; }



  // Identity Verification
  public string? IdentityNumber { get; private set; } = "";

  public string? IdentityImgUrl { get; set; } = "";

  public string? IdentityImgBackUrl { get; set; } = "";

  public bool? IsIdentityVerified { get; private set; } = false;

  // Online Status
  public bool IsOnline { get; private set; } = false;
  public DateTime? LastOnlineAt { get; private set; }

  // Soft Delete

  public bool IsDeleted { get; private set; } = false;
  public DateTime? DeletedAt { get; private set; }

  // Foreign Keys
  public string UserId { get; private set; } = "";

  public string FullName => $"{FirstName} {LastName}";
  private UserProfile() { }

  private UserProfile(string userId, string firstName, string lastName, ProfileType profileType)
    : base(Guid.NewGuid())
  {
    UserId = userId;
    FirstName = firstName;
    LastName = lastName;
    ProfileType = profileType;
  }

  public static Result<UserProfile> create(string userId, string firstName, string lastName, ProfileType profileType)
  {

    if (string.IsNullOrWhiteSpace(userId))
    {
      return UserProfileErrors.UserIdIsRequired;
    }
    if (string.IsNullOrWhiteSpace(firstName))
    {
      return UserProfileErrors.FirstNameIsRequired;
    }
    if (string.IsNullOrWhiteSpace(lastName))
    {
      return UserProfileErrors.LastNameIsRequired;
    }

    var UserProfile = new UserProfile(userId, firstName, lastName, profileType);

    return UserProfile;
  }

}