using Xunit;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Domain.UserProfiles.Tests;

public class UserProfileTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        var userId = "test-user-123";
        var firstName = "John";
        var lastName = "Doe";
        var profileType = ProfileType.Freelancer;

        var result = UserProfile.create(userId, firstName, lastName, profileType);

        Assert.True(result.IsSuccess);
        var userProfile = result.Value;
        Assert.Equal(userId, userProfile.UserId);
        Assert.Equal(firstName, userProfile.FirstName);
        Assert.Equal(lastName, userProfile.LastName);
        Assert.Equal(profileType, userProfile.ProfileType);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldReturnUserIdIsRequiredError()
    {
        var userId = "";
        var firstName = "John";
        var lastName = "Doe";
        var profileType = ProfileType.Freelancer;

        var result = UserProfile.create(userId, firstName, lastName, profileType);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserProfileErrors.UserIdIsRequired, result.TopError);
    }

    [Fact]
    public void Create_WithNullUserId_ShouldReturnUserIdIsRequiredError()
    {
        string? userId = null;
        var firstName = "John";
        var lastName = "Doe";
        var profileType = ProfileType.Freelancer;

        var result = UserProfile.create(userId!, firstName, lastName, profileType);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserProfileErrors.UserIdIsRequired, result.TopError);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldReturnFirstNameIsRequiredError()
    {
        var userId = "test-user-123";
        var firstName = "";
        var lastName = "Doe";
        var profileType = ProfileType.Freelancer;

        var result = UserProfile.create(userId, firstName, lastName, profileType);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserProfileErrors.FirstNameIsRequired, result.TopError);
    }

    [Fact]
    public void Create_WithEmptyLastName_ShouldReturnLastNameIsRequiredError()
    {
        var userId = "test-user-123";
        var firstName = "John";
        var lastName = "";
        var profileType = ProfileType.Freelancer;

        var result = UserProfile.create(userId, firstName, lastName, profileType);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserProfileErrors.LastNameIsRequired, result.TopError);
    }

    [Fact]
    public void FullName_ShouldReturnConcatenatedFirstAndLastName()
    {
        var userId = "test-user-123";
        var firstName = "John";
        var lastName = "Doe";
        var profileType = ProfileType.Freelancer;

        var createResult = UserProfile.create(userId, firstName, lastName, profileType);
        var userProfile = createResult.Value;

        var fullName = userProfile.FullName;

        Assert.Equal("John Doe", fullName);
    }

    [Fact]
    public void UpdateProfile_WithAllParameters_ShouldUpdateAllProperties()
    {
        var userProfile = CreateValidUserProfile();
        var newFirstName = "Jane";
        var newLastName = "Smith";
        var newTitle = "Senior Developer";
        var newBio = "Experienced developer";
        var newExperienceYear = ExperienceYear.FiveToTenYears;
        var newIsAvailable = false;
        var newProfileType = ProfileType.Client;
        var newPhoneNumber = "+1234567890";

        var result = userProfile.UpdateProfile(
            newFirstName,
            newLastName,
            newTitle,
            newBio,
            newExperienceYear,
            newIsAvailable,
            newProfileType,
            newPhoneNumber);

        Assert.True(result.IsSuccess);
        Assert.Equal(newFirstName, userProfile.FirstName);
        Assert.Equal(newLastName, userProfile.LastName);
        Assert.Equal(newTitle, userProfile.Title);
        Assert.Equal(newBio, userProfile.Bio);
        Assert.Equal(newExperienceYear, userProfile.ExperienceYear);
        Assert.Equal(newIsAvailable, userProfile.IsAvailable);
        Assert.Equal(newProfileType, userProfile.ProfileType);
        Assert.Equal(newPhoneNumber, userProfile.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WithNullParameters_ShouldNotChangeProperties()
    {
        var userProfile = CreateValidUserProfile();
        var originalFirstName = userProfile.FirstName;
        var originalLastName = userProfile.LastName;
        var originalTitle = userProfile.Title;
        var originalBio = userProfile.Bio;
        var originalExperienceYear = userProfile.ExperienceYear;
        var originalIsAvailable = userProfile.IsAvailable;
        var originalProfileType = userProfile.ProfileType;
        var originalPhoneNumber = userProfile.PhoneNumber;

        var result = userProfile.UpdateProfile(null, null, null, null, null, null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalFirstName, userProfile.FirstName);
        Assert.Equal(originalLastName, userProfile.LastName);
        Assert.Equal(originalTitle, userProfile.Title);
        Assert.Equal(originalBio, userProfile.Bio);
        Assert.Equal(originalExperienceYear, userProfile.ExperienceYear);
        Assert.Equal(originalIsAvailable, userProfile.IsAvailable);
        Assert.Equal(originalProfileType, userProfile.ProfileType);
        Assert.Equal(originalPhoneNumber, userProfile.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WithPartialParameters_ShouldOnlyUpdateProvidedProperties()
    {
        var userProfile = CreateValidUserProfile();
        var originalFirstName = userProfile.FirstName;
        var originalLastName = userProfile.LastName;
        var newTitle = "New Title";
        var newBio = "New Bio";

        var result = userProfile.UpdateProfile(null, null, newTitle, newBio, null, null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalFirstName, userProfile.FirstName);
        Assert.Equal(originalLastName, userProfile.LastName);
        Assert.Equal(newTitle, userProfile.Title);
        Assert.Equal(newBio, userProfile.Bio);
    }

    [Fact]
    public void UpdateSpecilization_WithValidId_ShouldUpdateSpecializationId()
    {
        var userProfile = CreateValidUserProfile();
        var specializationId = Guid.NewGuid();

        var result = userProfile.updateSpecilization(specializationId);

        Assert.True(result.IsSuccess);
        Assert.Equal(specializationId, userProfile.SpecializationId);
    }

    [Fact]
    public void UpdateIdentityInfo_WithAllParameters_ShouldUpdateIdentityProperties()
    {
        var userProfile = CreateValidUserProfile();
        var identityNumber = "ID123456";
        var dateOfBirth = new DateOnly(1990, 1, 1);
        var identityLocation = "New York";
        var identityName = "John Doe";

        var result = userProfile.UpdateIdentityInfo(identityNumber, dateOfBirth, identityLocation, identityName);

        Assert.True(result.IsSuccess);
        Assert.Equal(identityNumber, userProfile.IdentityNumber);
        Assert.Equal(dateOfBirth, userProfile.DateOfBirth);
        Assert.Equal(identityLocation, userProfile.IdentityLocation);
        Assert.Equal(identityName, userProfile.IdentityName);
    }

    [Fact]
    public void UpdateIdentityInfo_WithNullParameters_ShouldNotChangeProperties()
    {
        var userProfile = CreateValidUserProfile();
        var originalIdentityNumber = userProfile.IdentityNumber;
        var originalDateOfBirth = userProfile.DateOfBirth;
        var originalIdentityLocation = userProfile.IdentityLocation;
        var originalIdentityName = userProfile.IdentityName;

        var result = userProfile.UpdateIdentityInfo(null, null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalIdentityNumber, userProfile.IdentityNumber);
        Assert.Equal(originalDateOfBirth, userProfile.DateOfBirth);
        Assert.Equal(originalIdentityLocation, userProfile.IdentityLocation);
        Assert.Equal(originalIdentityName, userProfile.IdentityName);
    }

    [Fact]
    public void UpdateIdentityImages_WithValidUrls_ShouldUpdateImageUrls()
    {
        var userProfile = CreateValidUserProfile();
        var frontImageUrl = "www.example.com/front.jpg";
        var backImageUrl = "www.example.com/back.jpg";

        var result = userProfile.UpdateIdentityImages(frontImageUrl, backImageUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(frontImageUrl, userProfile.IdentityImgUrl);
        Assert.Equal(backImageUrl, userProfile.IdentityImgBackUrl);
    }

    [Fact]
    public void UpdateIdentityImages_WithNullUrls_ShouldNotUpdateImageUrls()
    {
        var userProfile = CreateValidUserProfile();
        var originalFrontImageUrl = userProfile.IdentityImgUrl;
        var originalBackImageUrl = userProfile.IdentityImgBackUrl;

        var result = userProfile.UpdateIdentityImages(null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalFrontImageUrl, userProfile.IdentityImgUrl);
        Assert.Equal(originalBackImageUrl, userProfile.IdentityImgBackUrl);
    }

    [Fact]
    public void UpdateIdentityImages_WithEmptyStrings_ShouldNotUpdateImageUrls()
    {
        var userProfile = CreateValidUserProfile();
        var originalFrontImageUrl = userProfile.IdentityImgUrl;
        var originalBackImageUrl = userProfile.IdentityImgBackUrl;

        var result = userProfile.UpdateIdentityImages("", "");

        Assert.True(result.IsSuccess);
        Assert.Equal(originalFrontImageUrl, userProfile.IdentityImgUrl);
        Assert.Equal(originalBackImageUrl, userProfile.IdentityImgBackUrl);
    }

    [Fact]
    public void VerifyIdentity_ShouldSetIsIdentityVerifiedToTrue()
    {
        var userProfile = CreateValidUserProfile();

        var result = userProfile.VerifyIdentity();

        Assert.True(result.IsSuccess);
        Assert.True(userProfile.IsIdentityVerified);
    }


    [Fact]
    public void UpdateIdentityInfo_WithPartialParameters_ShouldOnlyUpdateProvidedProperties()
    {
        var userProfile = CreateValidUserProfile();
        var originalIdentityNumber = userProfile.IdentityNumber;
        var newDateOfBirth = new DateOnly(1995, 5, 15);
        var newIdentityLocation = "Los Angeles";

        var result = userProfile.UpdateIdentityInfo(null, newDateOfBirth, newIdentityLocation, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalIdentityNumber, userProfile.IdentityNumber);
        Assert.Equal(newDateOfBirth, userProfile.DateOfBirth);
        Assert.Equal(newIdentityLocation, userProfile.IdentityLocation);
    }

    [Fact]
    public void UpdateIdentityImages_WithSingleUrl_ShouldUpdateOnlyProvidedUrl()
    {
        var userProfile = CreateValidUserProfile();
        var frontImageUrl = "https://example.com/front.jpg";
        var originalBackImageUrl = userProfile.IdentityImgBackUrl;

        var result = userProfile.UpdateIdentityImages(frontImageUrl, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(frontImageUrl, userProfile.IdentityImgUrl);
        Assert.Equal(originalBackImageUrl, userProfile.IdentityImgBackUrl);
    }

    private UserProfile CreateValidUserProfile()
    {
        var result = UserProfile.create("test-user-123", "John", "Doe", ProfileType.Freelancer);
        return result.Value;
    }
}