using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Jobs.Commands.DeleteJob;
using Kawadar.Domain.Common.Results;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class DeleteJobTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public DeleteJobTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryJobsRepository>().Clear();
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
            .WithSlug("test-job-to-delete")
            .Build();
        jobsRepo.Jobs.Add(job);
        return job.JobSlug;
    }

    [Fact]
    public async Task DeleteJob_ByOwner_ShouldSucceed_AndRemoveFromRepository()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new DeleteJobCommand(jobSlug);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        Assert.Empty(jobsRepo.Jobs);
    }

    [Fact]
    public async Task DeleteJob_ByNonOwner_ShouldFail()
    {
        // Arrange
        var (ownerId, ownerProfileId) = SeedActivatedClient();
        var jobSlug = SeedJob(ownerProfileId);

        // Seed a different client
        var (nonOwnerId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(nonOwnerId);

        var command = new DeleteJobCommand(jobSlug);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Unauthorized, result.TopError.Type);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        Assert.Single(jobsRepo.Jobs); // Job should still be there
    }

    [Fact]
    public async Task DeleteJob_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var (ownerId, ownerProfileId) = SeedActivatedClient();
        var jobSlug = SeedJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Do not set user

        var command = new DeleteJobCommand(jobSlug);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }

    [Fact]
    public async Task DeleteJob_NonExistentJob_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new DeleteJobCommand("non-existent-job-slug");

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }
}
