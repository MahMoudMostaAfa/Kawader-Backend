using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Application.Features.Jobs.Commands.CreateJob.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Tests.Common.Specilizations;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class CreateJobTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public CreateJobTests(SubcutaneousTestFixture fixture)
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

    // ───────── Helper: seed an activated, identity-verified client ─────────

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

    private Guid SeedSpecialization(string name = "Backend Development")
    {
        var specRepo = _fixture.Services.GetRequiredService<InMemorySpecilizationRepository>();
        var spec = SpecilizationFactory.CreateActive(name);
        specRepo.Specilizations.Add(spec);
        return spec.Id;
    }

    // ───────────────────── Fixed-Price Job: Happy Path ─────────────────────

    [Fact]
    public async Task CreateJob_FixedPrice_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new CreateJobCommand(
            Title: "Senior Backend Developer",
            Description: "Need an experienced .NET backend developer for API work.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 14,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateJob_FixedPrice_ShouldPersistInRepository()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new CreateJobCommand(
            Title: "React Native Developer",
            Description: "Build a cross-platform mobile application using React Native.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 30,
            ExperienceLevel: JobExperienceLevel.SeniorLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        Assert.Single(jobsRepo.Jobs);

        var job = jobsRepo.Jobs[0];
        Assert.Equal("React Native Developer", job.Title);
        Assert.Equal(JobType.FixedPrice, job.JobType);
        Assert.Equal(profileId, job.PostedById);
        Assert.Equal(specId, job.SpecilizationId);

        // Verify valid slug: non-empty, URL-safe (no spaces), contains slugified title prefix
        Assert.NotEmpty(job.JobSlug);
        Assert.DoesNotContain(" ", job.JobSlug);
        Assert.StartsWith("react-native-developer-", job.JobSlug);
    }

    // ───────────────────── Hourly Job: Happy Path ─────────────────────

    [Fact]
    public async Task CreateJob_Hourly_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new CreateJobCommand(
            Title: "DevOps Engineer Hourly",
            Description: "Ongoing DevOps support for CI/CD pipelines and infrastructure.",
            SpecilizationId: specId,
            JobType: JobType.Hourly,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 60,
            ExperienceLevel: JobExperienceLevel.ExpertLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        Assert.Single(jobsRepo.Jobs);
        Assert.Equal(JobType.Hourly, jobsRepo.Jobs[0].JobType);
    }

    // ───────────────── Job with Questions ─────────────────

    [Fact]
    public async Task CreateJob_WithQuestions_ShouldSucceed()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var questions = new List<CreateQuestionDto>
        {
            new("What is your approach to API design?", true),
            new("Do you have experience with microservices?", false)
        };

        var command = new CreateJobCommand(
            Title: "API Architect",
            Description: "Need an architect to design RESTful APIs for a large-scale system.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 21,
            ExperienceLevel: JobExperienceLevel.ExpertLevel,
            QuestionDtos: questions,
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var jobsRepo = _fixture.Services.GetRequiredService<InMemoryJobsRepository>();
        Assert.Equal(2, jobsRepo.Jobs[0].Questions.Count());
    }

    // ───────────────── Invalid Specialization ─────────────────

    [Fact]
    public async Task CreateJob_WithInvalidSpecialization_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var nonExistentSpecId = Guid.NewGuid();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new CreateJobCommand(
            Title: "Backend Developer",
            Description: "Need a .NET developer for backend work on our platform.",
            SpecilizationId: nonExistentSpecId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 14,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    // ───────────────── Unauthenticated User ─────────────────

    [Fact]
    public async Task CreateJob_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        // Do NOT call SetUser — FakeUser.Id remains null

        var command = new CreateJobCommand(
            Title: "Unauthenticated Job",
            Description: "This job should never be created because the user is not authenticated.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 14,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }

    // ───────────────── Banned User ─────────────────

    [Fact]
    public async Task CreateJob_AsBannedUser_ShouldFail()
    {
        // Arrange
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "banned@example.com",
            userName: "banned.user",
            password: "P@ss1234",
            emailConfirmed: true);

        // Create an activated + verified but BANNED profile
        var profile = UserProfileFactory.Builder()
            .AsClient()
            .Activated()
            .Banned()
            .WithUserId(identityUser.Id)
            .Build();
        profile.VerifyIdentity();
        usersRepo.Users.Add(profile);

        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(identityUser.Id);

        var command = new CreateJobCommand(
            Title: "Banned User Job",
            Description: "This should be blocked because the user is banned from the platform.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 14,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.Unauthorized", result.TopError.Code);
    }

    // ───────────────── Non-Activated User ─────────────────

    [Fact]
    public async Task CreateJob_AsNonActivatedUser_ShouldFail()
    {
        // Arrange
        var identityService = _fixture.Services.GetRequiredService<FakeIdentityService>();
        var usersRepo = _fixture.Services.GetRequiredService<InMemoryUsersRepository>();

        var identityUser = identityService.SeedUser(
            email: "notactive@example.com",
            userName: "notactive.user",
            password: "P@ss1234",
            emailConfirmed: true);

        // Create a profile that is NOT activated (no Activated() call)
        var profile = UserProfileFactory.Builder()
            .AsClient()
            .WithUserId(identityUser.Id)
            .Build();
        usersRepo.Users.Add(profile);

        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(identityUser.Id);

        var command = new CreateJobCommand(
            Title: "Non-Activated User Job",
            Description: "This should be blocked because the user account is not activated.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 14,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("UserAccount.NotActivated", result.TopError.Code);
    }

    // ───────────────── Edge Cases ─────────────────

    [Fact]
    public async Task CreateJob_WithDurationZero_ShouldFailValidation()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new CreateJobCommand(
            Title: "Invalid Duration Job",
            Description: "Duration is zero which is invalid.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 0,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: [],
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    [Fact]
    public async Task CreateJob_WithMoreThan5Questions_ShouldFailValidation()
    {
        // Arrange
        var (userId, _) = SeedActivatedClient();
        var specId = SeedSpecialization();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var questions = Enumerable.Range(1, 6).Select(i => new CreateQuestionDto($"Question {i}", false)).ToList();

        var command = new CreateJobCommand(
            Title: "Too Many Questions Job",
            Description: "Has 6 questions.",
            SpecilizationId: specId,
            JobType: JobType.FixedPrice,
            BudgetRange: BudgetRange.From1000To5000,
            HourlyRateRange: HourlyRateRange.From100To200,
            DurationInDays: 10,
            ExperienceLevel: JobExperienceLevel.MidLevel,
            QuestionDtos: questions,
            SkillIds: [],
            AttachmentFiles: null,
            AttachmentLinks: null);

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }
}

