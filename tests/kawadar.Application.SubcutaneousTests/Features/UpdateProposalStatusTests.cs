using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Proposals.Commands.UpdateProposalStatus;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.Proposals;
using Kawadar.Tests.Common.Specilizations;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class UpdateProposalStatusTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public UpdateProposalStatusTests(SubcutaneousTestFixture fixture)
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

    // ───────── Helpers ─────────

    private (string UserId, Guid ProfileId) SeedJobOwner()
    {
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "owner@example.com",
            userName: "job.owner",
            password: "P@ss1234",
            emailConfirmed: true);

        var profile = UserProfileFactory.CreateActivatedClient(identityUser.Id);
        profile.VerifyIdentity();
        usersRepo.Users.Add(profile);

        return (identityUser.Id, profile.Id);
    }

    private (string UserId, Guid ProfileId) SeedFreelancer()
    {
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "freelancer@example.com",
            userName: "freelancer.user",
            password: "P@ss1234",
            emailConfirmed: true);

        var profile = UserProfileFactory.CreateActivatedFreelancer(identityUser.Id);
        profile.VerifyIdentity();
        usersRepo.Users.Add(profile);

        return (identityUser.Id, profile.Id);
    }

    private Guid SeedJobAndProposal(Guid ownerProfileId, Guid freelancerProfileId)
    {
        var specRepo = _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>();
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();

        var spec = SpecilizationFactory.CreateActive();
        specRepo.Specilizations.Add(spec);

        var job = JobFactory.Builder()
            .WithPostedById(ownerProfileId)
            .WithSpecilizationId(spec.Id)
            .WithJobType(JobType.FixedPrice)
            .WithSlug("status-test-job-" + Guid.NewGuid().ToString()[..8])
            .Build();
        jobsRepo.Jobs.Add(job);

        var proposal = JobProposalFactory.Builder()
            .WithJobId(job.Id)
            .WithFreelancerId(freelancerProfileId)
            .WithProposalType(JobProposalType.OneTime)
            .WithAmount(1500m)
            .WithEstimatedDays(10)
            .Build();
        proposalsRepo.Proposals.Add(proposal);

        return proposal.Id;
    }

    // ───────────────── Exclude: Happy Path (Job Owner) ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Exclude_ByJobOwner_ShouldSucceed()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        Assert.Equal(JobProposalStatus.Excluded, proposalsRepo.Proposals[0].Status);
    }

    // ───────────────── Status Persists After Exclude ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Exclude_ShouldChangeStatusFromPending()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        Assert.Equal(JobProposalStatus.Pending, proposalsRepo.Proposals[0].Status);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JobProposalStatus.Excluded, proposalsRepo.Proposals[0].Status);
    }

    // ───────────────── Validation: Accepted Status Should Fail ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Accepted_ShouldFailValidation()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Accepted);

        // Act
        var result = await scope.Send(command);

        // Assert — the validator only allows Excluded status
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    // ───────────────── Validation: Rejected Status Should Fail ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Rejected_ShouldFailValidation()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Rejected);

        // Act
        var result = await scope.Send(command);

        // Assert — the validator only allows Excluded status
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    // ───────────────── Authorization: Freelancer (Non-Owner) Cannot Exclude ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Exclude_ByNonOwner_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Authenticate as the freelancer (not the job owner)
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.Unauthorized", result.TopError.Code);
    }

    // ───────────────── Authorization: Unauthenticated User ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Do NOT call SetUser — FakeUser.Id remains null

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }

    // ───────────────── Already-Withdrawn Proposal: Cannot Exclude ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Exclude_WithdrawnProposal_ShouldFail()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        // Manually set to Withdrawn first
        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        proposalsRepo.Proposals[0].UpdateState(JobProposalStatus.Withdrawn);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    // ───────────────── Already-Excluded Proposal: Cannot Exclude Again ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_Exclude_AlreadyExcluded_ShouldFail()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var (_, freelancerProfileId) = SeedFreelancer();
        var proposalId = SeedJobAndProposal(ownerProfileId, freelancerProfileId);

        // Manually set to Excluded first
        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        proposalsRepo.Proposals[0].UpdateState(JobProposalStatus.Excluded);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(proposalId, JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    // ───────────────── Non-Existent Proposal ─────────────────

    [Fact]
    public async Task UpdateProposalStatus_NonExistentProposal_ShouldFail()
    {
        // Arrange
        var (ownerUserId, _) = SeedJobOwner();
        SeedFreelancer();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new UpdateProposalStatusCommand(Guid.NewGuid(), JobProposalStatus.Excluded);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }
}
