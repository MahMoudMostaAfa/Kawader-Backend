using System.Text;
using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;
using Kawadar.Domain.Common.Results;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class UploadIdentityTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public UploadIdentityTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<FakeIdentityService>().Clear();
        _fixture.Services.GetRequiredService<FakeEventBus>().Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private (string UserId, Guid ProfileId) SeedClient(bool isVerified)
    {
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "client@example.com",
            userName: "client.user",
            password: "P@ss1234",
            emailConfirmed: true);

        var profile = UserProfileFactory.CreateActivatedClient(identityUser.Id);
        if (isVerified)
        {
            profile.VerifyIdentity();
        }
        
        usersRepo.Users.Add(profile);

        return (identityUser.Id, profile.Id);
    }

    private IFormFile CreateFakeFormFile(string fileName, string content = "fake-image-content")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    [Fact]
    public async Task UploadIdentity_WithValidFiles_ShouldSucceed_AndPublishEvents()
    {
        // Arrange
        var (userId, profileId) = SeedClient(isVerified: false);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var frontImage = CreateFakeFormFile("front.jpg");
        var backImage = CreateFakeFormFile("back.jpg");

        var command = new UploadIdentityCommand(frontImage, backImage);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var eventBus = _fixture.Services.GetRequiredService<FakeEventBus>();
        Assert.Equal(2, eventBus.PublishedMessages.Count);
        // It publishes UploadIdentityMessage and ProcessingIdentityDataMessage
    }

    [Fact]
    public async Task UploadIdentity_AlreadyVerified_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedClient(isVerified: true);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var frontImage = CreateFakeFormFile("front.jpg");
        var backImage = CreateFakeFormFile("back.jpg");

        var command = new UploadIdentityCommand(frontImage, backImage);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("UserProfile.IdentityAlreadyVerified", result.TopError.Code);
    }

    [Fact]
    public async Task UploadIdentity_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();

        var frontImage = CreateFakeFormFile("front.jpg");
        var backImage = CreateFakeFormFile("back.jpg");

        var command = new UploadIdentityCommand(frontImage, backImage);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
    
    [Fact]
    public async Task UploadIdentity_WithInvalidFileType_ShouldFailValidation()
    {
        // Arrange
        var (userId, _) = SeedClient(isVerified: false);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var bytes = Encoding.UTF8.GetBytes("fake-pdf-content");
        var stream = new MemoryStream(bytes);
        var invalidFile = new FormFile(stream, 0, stream.Length, "file", "document.pdf")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
        
        var validImage = CreateFakeFormFile("back.jpg");

        var command = new UploadIdentityCommand(invalidFile, validImage);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }
}
