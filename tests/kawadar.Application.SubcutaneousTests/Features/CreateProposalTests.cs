using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Proposals.Commands.CreateProposal;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.Specilizations;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class CreateProposalTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public CreateProposalTests(SubcutaneousTestFixture fixture)
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

    // ───────── Helpers: seed users and job ─────────

    /// <summary>
    /// Seeds a client who owns the job (the job poster).
    /// </summary>
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

    /// <summary>
    /// Seeds a freelancer who will submit the proposal.
    /// </summary>
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

    /// <summary>
    /// Seeds a FixedPrice job owned by the given profile.
    /// </summary>
    private Guid SeedFixedPriceJob(Guid ownerProfileId)
    {
        var specRepo = _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>();
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();

        var spec = SpecilizationFactory.CreateActive();
        specRepo.Specilizations.Add(spec);

        var job = JobFactory.Builder()
            .WithPostedById(ownerProfileId)
            .WithSpecilizationId(spec.Id)
            .WithJobType(JobType.FixedPrice)
            .WithSlug("fixed-job-" + Guid.NewGuid().ToString()[..8])
            .Build();

        jobsRepo.Jobs.Add(job);
        return job.Id;
    }

    /// <summary>
    /// Seeds an Hourly job owned by the given profile.
    /// </summary>
    private Guid SeedHourlyJob(Guid ownerProfileId)
    {
        var specRepo = _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>();
        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();

        var spec = SpecilizationFactory.CreateActive("DevOps");
        specRepo.Specilizations.Add(spec);

        var job = JobFactory.Builder()
            .WithPostedById(ownerProfileId)
            .WithSpecilizationId(spec.Id)
            .WithJobType(JobType.Hourly)
            .WithSlug("hourly-job-" + Guid.NewGuid().ToString()[..8])
            .Build();

        jobsRepo.Jobs.Add(job);
        return job.Id;
    }

    private const string ValidCoverLetter =
        "I have extensive experience in this domain and would love to contribute to this project. " +
        "My approach involves thorough analysis and iterative development.";

    // ───────────────────── OneTime Proposal: Happy Path ─────────────────────

    [Fact]
    public async Task CreateProposal_OneTime_ShouldSucceed()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedFixedPriceJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.OneTime,
            Amount: 1500m,
            EstimatedDays: 10,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        Assert.Single(proposalsRepo.Proposals);
        Assert.Equal(JobProposalType.OneTime, proposalsRepo.Proposals[0].ProposalType);
        Assert.Equal(1500m, proposalsRepo.Proposals[0].Amount);
    }

    // ───────────────────── Hourly Proposal: Happy Path ─────────────────────

    [Fact]
    public async Task CreateProposal_Hourly_ShouldSucceed()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedHourlyJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.Hourly,
            Amount: null,
            EstimatedDays: null,
            HourlyRate: 50,
            EstimatedHours: 20,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        Assert.Single(proposalsRepo.Proposals);
        Assert.Equal(JobProposalType.Hourly, proposalsRepo.Proposals[0].ProposalType);
        Assert.Equal(50, proposalsRepo.Proposals[0].HourlyRate);
        Assert.Equal(20, proposalsRepo.Proposals[0].EstimatedHours);
    }

    // ───────────────── Milestone-Based Proposal: Happy Path ─────────────────

    [Fact]
    public async Task CreateProposal_MilestoneBased_ShouldSucceed()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedFixedPriceJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var milestones = new List<MilestoneDto>
        {
            new() { Title = "Phase 1", Description = "Initial setup", Amount = 500m, DueDate = DateTime.UtcNow.AddDays(7) },
            new() { Title = "Phase 2", Description = "Core features", Amount = 1000m, DueDate = DateTime.UtcNow.AddDays(14) }
        };

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.MilestoneBased,
            Amount: null,
            EstimatedDays: null,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: milestones);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var proposalsRepo = _fixture.Services.GetRequiredService<InMemoryProposalsRepository>();
        Assert.Single(proposalsRepo.Proposals);

        var proposal = proposalsRepo.Proposals[0];
        Assert.Equal(JobProposalType.MilestoneBased, proposal.ProposalType);
        Assert.Equal(2, proposal.Milestones.Count());
    }

    // ───────────────── Own-Job Bidding Guard ─────────────────

    [Fact]
    public async Task CreateProposal_OnOwnJob_ShouldFail()
    {
        // Arrange
        var (ownerUserId, ownerProfileId) = SeedJobOwner();
        var jobId = SeedFixedPriceJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Authenticate as the job owner (same user)
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(ownerUserId);

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.OneTime,
            Amount: 1000m,
            EstimatedDays: 7,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Forbidden, result.TopError.Type);
    }

    // ───────────── Hourly Proposal on Fixed-Price Job (Type Mismatch) ─────────────

    [Fact]
    public async Task CreateProposal_Hourly_OnFixedPriceJob_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedFixedPriceJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.Hourly,
            Amount: null,
            EstimatedDays: null,
            HourlyRate: 40,
            EstimatedHours: 10,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    // ───────────── OneTime Proposal on Hourly Job (Type Mismatch) ─────────────

    [Fact]
    public async Task CreateProposal_OneTime_OnHourlyJob_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedHourlyJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.OneTime,
            Amount: 2000m,
            EstimatedDays: 15,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    // ───────────── MilestoneBased Proposal on Hourly Job (Type Mismatch) ─────────────

    [Fact]
    public async Task CreateProposal_MilestoneBased_OnHourlyJob_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var (freelancerUserId, _) = SeedFreelancer();
        var jobId = SeedHourlyJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(freelancerUserId);

        var milestones = new List<MilestoneDto>
        {
            new() { Title = "Phase 1", Description = "Setup", Amount = 300m, DueDate = DateTime.UtcNow.AddDays(5) }
        };

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.MilestoneBased,
            Amount: null,
            EstimatedDays: null,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: milestones);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    // ───────────── Unauthenticated User ─────────────

    [Fact]
    public async Task CreateProposal_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var (_, ownerProfileId) = SeedJobOwner();
        var jobId = SeedFixedPriceJob(ownerProfileId);

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Do NOT call SetUser — FakeUser.Id remains null

        var command = new CreateProposalCommand(
            JobId: jobId,
            CoverLetter: ValidCoverLetter,
            JobProposalType: JobProposalType.OneTime,
            Amount: 1000m,
            EstimatedDays: 7,
            HourlyRate: null,
            EstimatedHours: null,
            QuestionAnswerDtos: null,
            MilestoneDtos: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
}
