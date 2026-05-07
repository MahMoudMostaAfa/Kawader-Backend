using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Reviews.Commands.CreateReview;
using Kawadar.Domain.Reviews.Enums;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class CreateReviewTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public CreateReviewTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
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

    private (string ClientId, string FreelancerId, string FreelancerUserName, string ClientUserName, Guid ClientProfileId) SeedUsers()
    {
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var clientIdentity = identityService.SeedUser(
            email: "client@example.com",
            userName: "client.user",
            password: "P@ss1234",
            emailConfirmed: true);

        var freelancerIdentity = identityService.SeedUser(
            email: "freelancer@example.com",
            userName: "freelancer.user",
            password: "P@ss1234",
            emailConfirmed: true);

        var clientProfile = UserProfileFactory.CreateActivatedClient(clientIdentity.Id);
        clientProfile.VerifyIdentity();
        usersRepo.Users.Add(clientProfile);

        var freelancerProfile = UserProfileFactory.CreateActivatedFreelancer(freelancerIdentity.Id);
        freelancerProfile.VerifyIdentity();
        usersRepo.Users.Add(freelancerProfile);

        return (clientIdentity.Id, freelancerIdentity.Id, freelancerIdentity.UserName, clientIdentity.UserName, clientProfile.Id);
    }

    private string SeedJob(Guid clientId)
    {
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        var job = JobFactory.Builder()
            .WithPostedById(clientId)
            .WithSlug("test-job")
            .Build();
        jobsRepo.Jobs.Add(job);
        return job.JobSlug;
    }

    [Fact]
    public async Task CreateReview_ByClientForFreelancer_ShouldSucceed_AndMapToClientFreelancerType()
    {
        // Arrange
        var (clientId, _, freelancerUserName, _, clientProfileId) = SeedUsers();
        var jobSlug = SeedJob(clientProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(clientId);

        var command = new CreateReviewCommand(
            jobSlug: jobSlug,
            RevieweeUserName: freelancerUserName,
            rating: 4.5f,
            comment: "Great job!");

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var reviewsRepo = _fixture.Services.GetRequiredService<InMemoryReviewRepository>();
        Assert.Single(reviewsRepo.Reviews);
        var review = reviewsRepo.Reviews.First();
        Assert.Equal(ReviewType.ClientFreelancer, review.ReviewType);
        Assert.Equal(4.5f, review.Rating);
        Assert.Equal("Great job!", review.Comment);
    }

    [Fact]
    public async Task CreateReview_ByFreelancerForClient_ShouldSucceed_AndMapToFreelancerClientType()
    {
        // Arrange
        var (_, freelancerId, _, clientUserName, clientProfileId) = SeedUsers();
        var jobSlug = SeedJob(clientProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerId);

        var command = new CreateReviewCommand(
            jobSlug: jobSlug,
            RevieweeUserName: clientUserName,
            rating: 5.0f,
            comment: "Great client!");

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var reviewsRepo = _fixture.Services.GetRequiredService<InMemoryReviewRepository>();
        Assert.Single(reviewsRepo.Reviews);
        var review = reviewsRepo.Reviews.First();
        Assert.Equal(ReviewType.FreelancerClient, review.ReviewType);
        Assert.Equal(5.0f, review.Rating);
        Assert.Equal("Great client!", review.Comment);
    }

    [Fact]
    public async Task CreateReview_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var (_, _, freelancerUserName, _, clientProfileId) = SeedUsers();
        var jobSlug = SeedJob(clientProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();

        var command = new CreateReviewCommand(
            jobSlug: jobSlug,
            RevieweeUserName: freelancerUserName,
            rating: 4.5f,
            comment: "Great job!");

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
}
