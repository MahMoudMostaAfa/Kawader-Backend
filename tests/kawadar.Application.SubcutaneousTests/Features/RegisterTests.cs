using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Auth.Commands.Register;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class RegisterTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public RegisterTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        // Clear all in-memory repositories and fakes between tests
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryJobsRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryProposalsRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemorySkillRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryReviewRepository>().Clear();
        _fixture.Services.GetRequiredService<FakeIdentityService>().Clear();
        _fixture.Services.GetRequiredService<FakeEmailService>().Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ───────────────────────── Happy Path ─────────────────────────

    [Fact]
    public async Task Register_WithValidFreelancer_ShouldSucceed()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var command = new RegisterCommand(
            "Ahmed",
            "Hassan",
            "ahmed@example.com",
            "P@ssw0rd123",
            ProfileType.Freelancer);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Register_WithValidClient_ShouldSucceed()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var command = new RegisterCommand(
            "Sara",
            "Ali",
            "sara@example.com",
            "P@ssw0rd123",
            ProfileType.Client);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldCreateUserProfile()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();
        var command = new RegisterCommand(
            "Mohamed",
            "Youssef",
            "mohamed@example.com",
            "P@ssw0rd123",
            ProfileType.Freelancer);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(usersRepo.Users);

        var profile = usersRepo.Users[0];
        Assert.Equal("Mohamed", profile.FirstName);
        Assert.Equal("Youssef", profile.LastName);
        Assert.Equal(ProfileType.Freelancer, profile.ProfileType);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldCaptureWelcomeEmail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var emailService = _fixture.Services.GetRequiredService<FakeEmailService>();
        var command = new RegisterCommand(
            "Fatma",
            "Ibrahim",
            "fatma@example.com",
            "P@ssw0rd123",
            ProfileType.Client);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(emailService.SentEmails);
        Assert.Equal("fatma@example.com", emailService.SentEmails[0].To);
        Assert.Equal("Welcome to Kawadar", emailService.SentEmails[0].Subject);
        Assert.Contains("Fatma", emailService.SentEmails[0].Body);
    }

    // ───────────────────── Admin Registration Blocked ─────────────────────

    [Fact]
    public async Task Register_AsAdmin_ShouldFail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var command = new RegisterCommand(
            "Admin",
            "User",
            "admin@example.com",
            "P@ssw0rd123",
            ProfileType.Admin);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("UserProfile.InvalidProfileType", result.TopError.Code);
    }

    [Fact]
    public async Task Register_AsAdmin_ShouldNotCreateUserProfile()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();
        var command = new RegisterCommand(
            "Admin",
            "User",
            "admin@example.com",
            "P@ssw0rd123",
            ProfileType.Admin);

        // Act
        await scope.Send(command);

        // Assert
        Assert.Empty(usersRepo.Users);
    }

    [Fact]
    public async Task Register_AsAdmin_ShouldNotSendEmail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();
        var emailService = _fixture.Services.GetRequiredService<FakeEmailService>();
        var command = new RegisterCommand(
            "Admin",
            "User",
            "admin@example.com",
            "P@ssw0rd123",
            ProfileType.Admin);

        // Act
        await scope.Send(command);

        // Assert
        Assert.Empty(emailService.SentEmails);
    }
}
