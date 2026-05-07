using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class UpdateProfileTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public UpdateProfileTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<FakeIdentityService>().Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private (string UserId, Guid ProfileId) SeedActivatedClient()
    {
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "client@example.com",
            userName: "client.user",
            password: "P@ss1234",
            emailConfirmed: true);

        var profile = UserProfileFactory.CreateActivatedClient(identityUser.Id);
        profile.VerifyIdentity();
        usersRepo.Users.Add(profile);

        return (identityUser.Id, profile.Id);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateProfileCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Title: "Senior Developer",
            Bio: "Experienced developer.",
            ExperienceYear: ExperienceYear.FiveToTenYears,
            IsAvailable: true,
            ProfileType: ProfileType.Freelancer,
            PhoneNumber: "01012345678"
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess, result.IsError ? result.TopError.Code + " - " + result.TopError.Description : "");

        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();
        var profile = usersRepo.Users.First(u => u.Id == profileId);
        
        Assert.Equal("Jane", profile.FirstName);
        Assert.Equal("Doe", profile.LastName);
        Assert.Equal("Senior Developer", profile.Title);
        Assert.Equal("Experienced developer.", profile.Bio);
        Assert.Equal(ExperienceYear.FiveToTenYears, profile.ExperienceYear);
        Assert.True(profile.IsAvailable);
        Assert.Equal(ProfileType.Freelancer, profile.ProfileType);
        Assert.Equal("01012345678", profile.PhoneNumber);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidData_ShouldFailValidation()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateProfileCommand(
            FirstName: new string('A', 51), // Too long
            LastName: null,
            Title: null,
            Bio: null,
            ExperienceYear: null,
            IsAvailable: null,
            ProfileType: null,
            PhoneNumber: "invalid-phone"
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    [Fact]
    public async Task UpdateProfile_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();

        var command = new UpdateProfileCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Title: "Senior Developer",
            Bio: "Experienced developer.",
            ExperienceYear: ExperienceYear.FiveToTenYears,
            IsAvailable: true,
            ProfileType: ProfileType.Freelancer,
            PhoneNumber: "01012345678"
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
}
