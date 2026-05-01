using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Jobs.Commands.UpdateJob;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class UpdateJobTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public UpdateJobTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryJobsRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>().Clear();
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

    private string SeedJob(Guid clientId)
    {
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        var job = JobFactory.Builder()
            .WithPostedById(clientId)
            .WithTitle("Original Title")
            .WithDescription("Original Description")
            .WithSlug("test-job-to-update")
            .Build();
        jobsRepo.Jobs.Add(job);
        return job.JobSlug;
    }

    [Fact]
    public async Task UpdateJob_ByOwner_ShouldSucceed_AndReflectChanges()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: "Updated Title",
            Description: "Updated Description",
            SpecilizationId: null,
            JobType: JobType.Hourly,
            BudgetRange: null,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 30,
            ExperienceLevel: JobExperienceLevel.ExpertLevel);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        var job = jobsRepo.Jobs.First();
        Assert.Equal("Updated Title", job.Title);
        Assert.Equal("Updated Description", job.Description);
        Assert.Equal(JobType.Hourly, job.JobType);
        Assert.Equal(HourlyRateRange.From100To200, job.HourlyRateRange);
        Assert.Equal(30, job.DurationInDays);
        Assert.Equal(JobExperienceLevel.ExpertLevel, job.ExperienceLevel);
    }

    [Fact]
    public async Task UpdateJob_WithInvalidTitle_ShouldFailValidation()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: "A", // Less than 5 characters
            Description: null,
            SpecilizationId: null,
            JobType: null,
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: null,
            ExperienceLevel: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    [Fact]
    public async Task UpdateJob_ByNonOwner_ShouldFail()
    {
        // Arrange
        var (ownerId, ownerProfileId) = SeedActivatedClient();
        var jobSlug = SeedJob(ownerProfileId);

        var (nonOwnerId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(nonOwnerId);

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: "Hacked Title",
            Description: null,
            SpecilizationId: null,
            JobType: null,
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: null,
            ExperienceLevel: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Unauthorized, result.TopError.Type);
    }

    [Fact]
    public async Task UpdateJob_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var (ownerId, ownerProfileId) = SeedActivatedClient();
        var jobSlug = SeedJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: "Hacked Title",
            Description: null,
            SpecilizationId: null,
            JobType: null,
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: null,
            ExperienceLevel: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }

    [Fact]
    public async Task UpdateJob_NonExistentJob_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateJobCommand(
            Slug: "non-existent-slug",
            Title: "New Title",
            Description: null,
            SpecilizationId: null,
            JobType: null,
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: null,
            ExperienceLevel: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Job.NotFound", result.TopError.Code);
    }

    [Fact]
    public async Task UpdateJob_WithZeroDuration_ShouldFailValidation()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId, "test-job-to-update");

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: null,
            Description: null,
            JobType: null,
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: 0,
            ExperienceLevel: null,
            SpecilizationId: null
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    [Fact]
    public async Task UpdateJob_WithInvalidEnum_ShouldFailValidation()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId, "test-job-to-update");

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new UpdateJobCommand(
            Slug: jobSlug,
            Title: null,
            Description: null,
            JobType: (JobType)999, // Invalid
            BudgetRange: null,
            HourlyRateRange: null,
            DurationInDays: null,
            ExperienceLevel: null,
            SpecilizationId: null
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }
}
