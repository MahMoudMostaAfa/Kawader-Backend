using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.Mappers;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class ProfileManagmentMapperTests
{
  private readonly IMapper _mapper;

  public ProfileManagmentMapperTests()
  {
    var config = new MapperConfiguration(cfg => 
    {
        cfg.AddProfile<UserProfileToUserProfileDtoProfile>();
    }, NullLoggerFactory.Instance);

    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapUserProfileDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var userProfileResult = UserProfile.create("identity-id", "Salma", "Adel", ProfileType.Freelancer);
    Assert.True(userProfileResult.IsSuccess);

    var userProfile = userProfileResult.Value;
    var specId = Guid.NewGuid();
    var bannedUntil = DateTime.UtcNow.AddDays(7);

    userProfile.UpdateProfile("Salma", "Adel", "Senior Engineer", "Backend specialist", ExperienceYear.FiveToTenYears, true, null, "01000111");
    userProfile.UpdateProfilePicture("https://cdn/profiles/salma.png");
    userProfile.updateSpecilization(specId);
    userProfile.VerifyIdentity();
    userProfile.Ban(bannedUntil);
    userProfile.Delete();
    SetPrivateProperty(userProfile, nameof(UserProfile.IsOnline), true);

    var user = new UserDto
    {
      Id = "identity-id",
      Email = "salma@kawadar.dev",
      UserName = "salma_adel",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<UserProfileDto>((userProfile, user));

    // Assert
    Assert.Equal(userProfile.FirstName, result.FirstName);
    Assert.Equal(userProfile.LastName, result.LastName);
    Assert.Equal(userProfile.Title, result.Title);
    Assert.Equal(userProfile.Bio, result.Bio);
    Assert.Equal(userProfile.ExperienceYear, result.ExperienceYear);
    Assert.Equal(userProfile.ProfilePictureUrl, result.ProfilePictureUrl);
    Assert.Equal(userProfile.VideoLink, result.VideoLink);
    Assert.Equal(userProfile.PhoneNumber, result.PhoneNumber);
    Assert.Equal(userProfile.IsAvailable, result.IsAvailable);
    Assert.Equal(userProfile.IsActivated, result.IsActivated);
    Assert.Equal(userProfile.ActivatedAt, result.ActivatedAt);
    Assert.Equal(userProfile.IsOnline, result.IsOnline);
    Assert.Equal(userProfile.IsIdentityVerified, result.IsIdentityVerified);
    Assert.Equal(user.UserName, result.UserName);
    Assert.Equal(user.Email, result.Email);
    Assert.Equal(userProfile.ProfileType, result.ProfileType);
    Assert.Equal(userProfile.SpecializationId, result.specilizationId);
    Assert.Equal(userProfile.IsBanned, result.IsBanned);
    Assert.Equal(userProfile.BannedUntil, result.BannedUntil);
    Assert.Equal(userProfile.IsDeleted, result.IsDeleted);
    Assert.Empty(result.skills);
  }

  [Fact]
  public void MapUserProfileDto_NullUser_MapsIdentityFieldsAsNull()
  {
    // Arrange
    var userProfileResult = UserProfile.create("identity-id", "Mina", "Sami", ProfileType.Client);
    Assert.True(userProfileResult.IsSuccess);

    // Act
    var result = _mapper.Map<UserProfileDto>((userProfileResult.Value, (UserDto)null!));

    // Assert
    Assert.Equal(userProfileResult.Value.FirstName, result.FirstName);
    Assert.Equal(userProfileResult.Value.LastName, result.LastName);
    Assert.Null(result.UserName);
    Assert.Null(result.Email);
  }

  private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
  {
    var property = typeof(TTarget).GetProperty(propertyName);
    Assert.NotNull(property);
    property!.SetValue(target, value);
  }
}
