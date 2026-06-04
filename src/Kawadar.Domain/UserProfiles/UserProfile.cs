using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Domain.UserProfiles;

public class UserProfile : AuditableEntity
{
  public ProfileType ProfileType { get; private set; } = ProfileType.Freelancer;
  // Personal Information
  public string FirstName { get; private set; } = "";
  public string LastName { get; private set; } = "";





  public string? Title
  { get; private set; } = "";

  public string? Bio { get; private set; } = "";
  public ExperienceYear ExperienceYear { get; private set; } = ExperienceYear.LessThanOneYear;
  public string? ProfilePictureUrl { get; private set; } = "";
  public string? VideoLink { get; private set; } = "";
  public string? PhoneNumber { get; private set; } = "";


  // specialization
  public Specilization? Specialization { get; private set; }
  public Guid? SpecializationId { get; private set; }



  // availability Status
  public bool IsAvailable { get; private set; } = true;



  // Ban Status
  public bool IsBanned { get; private set; } = false;
  public DateTime? BannedUntil { get; private set; }

  // Activation Status
  public bool IsActivated { get; private set; } = false;
  public DateTime? ActivatedAt { get; private set; }



  // Identity Verification
  public string? IdentityNumber { get; private set; } = "";

  public DateOnly? DateOfBirth { get; private set; }

  public string? IdentityLocation { get; private set; } = "";

  public string? IdentityName { get; private set; } = "";
  public string? IdentityImgUrl { get; set; } = "";

  public string? IdentityImgBackUrl { get; set; } = "";

  public bool? IsIdentityVerified { get; private set; } = false;

  // Online Status
  public bool IsOnline { get; private set; } = false;
  public DateTime? LastOnlineAt { get; private set; }

  // Soft Delete

  public bool IsDeleted { get; private set; } = false;
  public DateTime? DeletedAt { get; private set; }
  public DateTime? ScheduledDeletionAt { get; private set; }

  // Foreign Keys
  public string UserId { get; private set; } = "";

  public IReadOnlyCollection<Skill> Skills { get; private set; } = [];
  public IReadOnlyCollection<Review> Reviews { get; private set; } = [];

  public string FullName => $"{FirstName} {LastName}";

  public string TextToEmbed => String.Join("", new[]
  {
    "Bio: ", Bio, "\n",
    "Title: ", Title, "\n",
    "Experience Year: ", ExperienceYear.ToString(), "\n",
    "Specialization: ", Specialization?.Name ?? "N/A", "\n",
    "Skills: ", Skills != null && Skills.Any() ? string.Join(", ", Skills.Select(s => s.Name)) : "N/A", "\n",
  });
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



  public Result<Updated> UpdateProfile(string? firstName, string? lastName, string? title, string? bio, ExperienceYear? experienceYear, bool? isAvailable, ProfileType? profileType, string? phoneNumber)
  {
    if (!string.IsNullOrWhiteSpace(firstName))
    {
      FirstName = firstName;
    }
    if (!string.IsNullOrWhiteSpace(lastName))
    {
      LastName = lastName;
    }
    if (!string.IsNullOrWhiteSpace(title))
    {
      Title = title;
    }
    if (!string.IsNullOrWhiteSpace(bio))
    {
      Bio = bio;
    }
    if (experienceYear.HasValue)
    {
      ExperienceYear = experienceYear.Value;
    }
    if (isAvailable.HasValue)
    {
      IsAvailable = isAvailable.Value;
    }
    if (profileType.HasValue)
    {
      ProfileType = profileType.Value;
    }
    if (!string.IsNullOrWhiteSpace(phoneNumber))
    {
      PhoneNumber = phoneNumber;
    }

    CheckProfileIsComplete();

    return Result.Updated;
  }

  public Result<Updated> updateSpecilization(Guid specilizationId)
  {
    SpecializationId = specilizationId;
    return Result.Updated;
  }
  public Result<Updated> UpdateIdentityInfo(string? identityNumber, DateOnly? dateOfBirth, string? identityLocation, string? identityName)
  {
    if (!string.IsNullOrWhiteSpace(identityNumber))
    {
      IdentityNumber = identityNumber;
    }
    if (dateOfBirth.HasValue)
    {
      DateOfBirth = dateOfBirth.Value;
    }
    if (!string.IsNullOrWhiteSpace(identityLocation))
    {
      IdentityLocation = identityLocation;
    }
    if (!string.IsNullOrWhiteSpace(identityName))
    {
      IdentityName = identityName;
    }

    IsIdentityVerified = true;

    return Result.Updated;
  }



  public Result<Updated> UpdateIdentityImages(string? frontImageUrl, string? backImageUrl)
  {
    if (!string.IsNullOrWhiteSpace(frontImageUrl))
    {
      IdentityImgUrl = frontImageUrl;
    }
    if (!string.IsNullOrWhiteSpace(backImageUrl))
    {
      IdentityImgBackUrl = backImageUrl;
    }

    return Result.Updated;
  }

  public Result<Updated> VerifyIdentity()
  {
    IsIdentityVerified = true;
    return Result.Updated;
  }

  public Result<Deleted> Delete()
  {
    IsDeleted = true;
    return Result.Deleted;
  }

  public Result<Success> Ban(DateTime bannedUntil)
  {
    IsBanned = true;
    BannedUntil = bannedUntil;
    return Result.Success;
  }

  public Result<Updated> UpdateProfilePicture(string profilePictureUrl)
  {
    if (string.IsNullOrWhiteSpace(profilePictureUrl))
    {
      return UserProfileErrors.ProfilePictureUrlIsRequired;
    }

    ProfilePictureUrl = profilePictureUrl;

    CheckProfileIsComplete();
    return Result.Updated;
  }

  private void CheckProfileIsComplete()
  {
    // This method can be used to check if the profile is complete based on required fields
    // For example, you can check if FirstName, LastName, Title, Bio, ExperienceYear, and ProfilePictureUrl are not null or empty
    if (!string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        !string.IsNullOrWhiteSpace(Title) &&
        !string.IsNullOrWhiteSpace(Bio) &&
         !string.IsNullOrWhiteSpace(PhoneNumber) &&
        !string.IsNullOrWhiteSpace(ProfilePictureUrl))
    {
      IsActivated = true;
      ActivatedAt = DateTime.UtcNow;
    }
    else
    {
      IsActivated = false;
      ActivatedAt = null;
    }
  }

  public Result<Updated> MarkAsDeleted()
  {
    if (IsDeleted)
      return UserProfileErrors.AccountAlreadyMarkedForDeletion;

    IsDeleted = true;
    DeletedAt = DateTime.UtcNow;
    ScheduledDeletionAt = DateTime.UtcNow.AddMonths(1);
    IsOnline = false;
    IsAvailable = false;
    return Result.Updated;
  }

  public Result<Updated> CancelDeletion()
  {
    IsDeleted = false;
    DeletedAt = null;
    ScheduledDeletionAt = null;
    return Result.Updated;
  }


  public Result<Updated> UpdateOnlineStatus(bool isOnline, DateTime? lastOnlineAt = null)
  {

    IsOnline = isOnline;
    LastOnlineAt = lastOnlineAt ?? DateTime.UtcNow;
    return Result.Updated;
  }
}