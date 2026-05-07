using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Application.Features.Admins.Mapper;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class AdminsMapperTests
{
  private readonly IMapper _mapper;

  public AdminsMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<AdminMapper>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapAdminDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var userProfile = CreateUserProfile();
    SetPrivateProperty(userProfile, nameof(UserProfile.IsOnline), true);
    userProfile.Delete();

    var user = new UserDto
    {
      Id = "identity-1",
      UserName = "john_doe",
      Email = "john@kawadar.dev",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<AdminDto>((userProfile, user));

    // Assert
    Assert.Equal(userProfile.FirstName, result.FirstName);
    Assert.Equal(userProfile.LastName, result.LastName);
    Assert.Equal(userProfile.IsOnline, result.IsOnline);
    Assert.Equal(user.UserName, result.UserName);
    Assert.Equal(user.Email, result.Email);
    Assert.Equal(userProfile.IsDeleted, result.IsDeleted);
  }

  [Fact]
  public void MapBriefUserProfileDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var userProfile = CreateUserProfile();
    SetPrivateProperty(userProfile, nameof(UserProfile.IsOnline), true);
    userProfile.Ban(DateTime.UtcNow.AddDays(2));

    var user = new UserDto
    {
      Id = "identity-2",
      UserName = "freelancer_1",
      Email = "freelancer@kawadar.dev",
    };

    // Act
    var result = _mapper.Map<BriefUserProfileDto>((userProfile, user));

    // Assert
    Assert.Equal(userProfile.FullName, result.fullName);
    Assert.Equal(userProfile.IsOnline, result.IsOnline);
    Assert.Equal(user.UserName, result.UserName);
    Assert.Equal(userProfile.IsDeleted, result.IsDeleted);
    Assert.Equal(userProfile.ProfileType, result.profileType);
    Assert.Equal(userProfile.IsBanned, result.IsBanned);
  }

  [Fact]
  public void MapAdminDto_NullUser_MapsUserFieldsAsNull()
  {
    // Arrange
    var userProfile = CreateUserProfile();

    // Act
    var result = _mapper.Map<AdminDto>((userProfile, (UserDto)null!));

    // Assert
    Assert.Equal(userProfile.FirstName, result.FirstName);
    Assert.Equal(userProfile.LastName, result.LastName);
    Assert.Null(result.UserName);
    Assert.Null(result.Email);
  }

  private static UserProfile CreateUserProfile()
  {
    var result = UserProfile.create("user-id", "John", "Doe", ProfileType.Client);
    Assert.True(result.IsSuccess);
    return result.Value;
  }

  private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
  {
    var property = typeof(TTarget).GetProperty(propertyName);
    Assert.NotNull(property);
    property!.SetValue(target, value);
  }
}
