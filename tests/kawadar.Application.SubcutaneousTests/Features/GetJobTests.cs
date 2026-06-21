using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;
using Kawadar.Application.Features.Jobs.Queries.GetJobs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class GetJobTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public GetJobTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemoryJobsRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>().Clear();
        _fixture.Services.GetRequiredService<FakeIdentityService>().Clear();
        _fixture.Services.GetRequiredService<InMemoryJobViewRepository>().Clear();
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

    private string SeedJob(Guid clientId, string title = "Senior Backend Developer", JobType jobType = JobType.FixedPrice)
    {
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        var slug = title.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString()[..8];
        var job = JobFactory.Builder()
            .WithPostedById(clientId)
            .WithTitle(title)
            .WithSlug(slug)
            .WithJobType(jobType)
            .Build();
        jobsRepo.Jobs.Add(job);
        return job.JobSlug;
    }

    [Fact]
    public async Task GetJobBySlug_ShouldReturnCorrectDto()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var jobSlug = SeedJob(profileId, "React Developer");

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var query = new GetJobBySlugQuery(jobSlug);

        // Act
        var result = await scope.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("React Developer", result.Value.Title);
        Assert.Equal(jobSlug, result.Value.JobSlug);
    }

    [Fact]
    public async Task GetJobBySlug_NonExistent_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var query = new GetJobBySlugQuery("non-existent-slug");

        // Act
        var result = await scope.Send(query);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    [Fact]
    public async Task GetJobs_ShouldReturnPaginatedList()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        SeedJob(profileId, "Job 1");
        SeedJob(profileId, "Job 2");
        SeedJob(profileId, "Job 3");

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var query = new GetJobsQuery(
            Search: null,
            MaxProposalCount: null,
            SpecilizationId: null,
            JobType: null,
            ExperienceLevel: null,
            BudgetRange: null,
            HourlyRateRange: null,
            SkillIds: null,
            Page: 1,
            PageSize: 2,
            SortBy: "newest");

        // Act
        var result = await scope.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(1, result.Value.PageNumber);
    }

    [Fact]
    public async Task GetJobs_WithFilter_ShouldReturnFilteredList()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        SeedJob(profileId, "Fixed Job", JobType.FixedPrice);
        SeedJob(profileId, "Hourly Job 1", JobType.Hourly);
        SeedJob(profileId, "Hourly Job 2", JobType.Hourly);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var query = new GetJobsQuery(
            Search: null,
            MaxProposalCount: null,
            SpecilizationId: null,
            JobType: JobType.Hourly,
            ExperienceLevel: null,
            BudgetRange: null,
            HourlyRateRange: null,
            SkillIds: null,
            Page: 1,
            PageSize: 10,
            SortBy: "newest");

        // Act
        var result = await scope.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.All(result.Value.Items, j => Assert.Equal(JobType.Hourly, j.JobType));
    }

    [Fact]
    public async Task GetJobs_Unauthenticated_ShouldFail()
    {
        // Arrange
        var query = new GetJobsQuery(
            Search: null,
            MaxProposalCount: null,
            SpecilizationId: null,
            JobType: null,
            ExperienceLevel: null,
            BudgetRange: null,
            HourlyRateRange: null,
            SkillIds: null,
            Page: 1,
            PageSize: 10,
            SortBy: "newest");

        await using var scope = _fixture.Services.CreateAsyncScope();

        // Act
        var result = await scope.Send(query);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
}
