using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Tests.Common.UserProfiles;

public static class UserProfileFactory
{
    public static UserProfileBuilder Builder() => new();

    public static UserProfile CreateActivatedClient(string userId = "")
        => Builder().AsClient().Activated().WithUserId(userId).Build();

    public static UserProfile CreateActivatedFreelancer(string userId = "")
        => Builder().AsFreelancer().Activated().WithUserId(userId).Build();

    public static UserProfile CreateBannedClient(string userId = "")
        => Builder().AsClient().Banned().WithUserId(userId).Build();

    public static UserProfile CreateDeletedFreelancer(string userId = "")
        => Builder().AsFreelancer().Deleted().WithUserId(userId).Build();
}

public sealed class UserProfileBuilder
{
    private string _userId = "";
    private string _firstName = "Test";
    private string _lastName = "User";
    private ProfileType _profileType = ProfileType.Freelancer;
    private bool _activate = false;
    private bool _ban = false;
    private bool _delete = false;

    public UserProfileBuilder WithUserId(string value)
    {
        _userId = string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString() : value;
        return this;
    }

    public UserProfileBuilder WithFirstName(string value) { _firstName = value; return this; }
    public UserProfileBuilder WithLastName(string value) { _lastName = value; return this; }

    public UserProfileBuilder AsClient() { _profileType = ProfileType.Client; return this; }
    public UserProfileBuilder AsFreelancer() { _profileType = ProfileType.Freelancer; return this; }
    public UserProfileBuilder AsAdmin() { _profileType = ProfileType.Admin; return this; }

    public UserProfileBuilder Activated() { _activate = true; return this; }
    public UserProfileBuilder Banned() { _ban = true; return this; }
    public UserProfileBuilder Deleted() { _delete = true; return this; }

    public UserProfile Build()
    {
        if (string.IsNullOrWhiteSpace(_userId))
            _userId = Guid.NewGuid().ToString();

        var result = UserProfile.create(_userId, _firstName, _lastName, _profileType);
        if (result.IsError)
            throw new InvalidOperationException($"Could not build UserProfile: {result.TopError.Code} - {result.TopError.Description}");

        var profile = result.Value;

        if (_activate)
        {
            // Fill required fields so CheckProfileIsComplete sets IsActivated = true
            profile.UpdateProfile(
                firstName: _firstName,
                lastName: _lastName,
                title: "Senior Developer",
                bio: "Experienced developer with 10+ years.",
                experienceYear: ExperienceYear.FiveToTenYears,
                isAvailable: true,
                profileType: _profileType,
                phoneNumber: "+201234567890");
            profile.UpdateProfilePicture("https://fake-storage/avatars/profile.png");
        }

        if (_ban)
            profile.Ban(DateTime.UtcNow.AddDays(30));

        if (_delete)
            profile.MarkAsDeleted();

        return profile;
    }
}
