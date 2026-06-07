using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using kawadar.Application.SubcutaneousTests.Common.Helpers;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner;
using Kawadar.Application.Features.Skills.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
using Kawadar.Tests.Common.Jobs;
using Kawadar.Tests.Common.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests.Features;

public class SkillAssignmentTests : IClassFixture<SubcutaneousTestFixture>, IAsyncLifetime
{
    private readonly SubcutaneousTestFixture _fixture;

    public SkillAssignmentTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _fixture.Services.GetRequiredService<InMemoryUsersRepository>().Clear();
        _fixture.Services.GetRequiredService<InMemorySkillRepository>().Clear();
        _fixture.Services.GetRequiredService<FakeIdentityService>().Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private (string UserId, Guid ProfileId) SeedActivatedFreelancer()
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

    private Guid SeedSkill(string name)
    {
        var skillRepo = _fixture.Services.GetRequiredService<InMemorySkillRepository>();
        var skill = JobFactory.CreateSkill(name);
        skillRepo.Skills.Add(skill);
        return skill.Id;
    }

    [Fact]
    public async Task AddSkillsToFreelancer_PredefinedSkill_ShouldSucceed()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedFreelancer();
        var skillId = SeedSkill("C#");

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new AddSkillsToFreelacnerCommand(
            Skills: [
                new CreateFreelancerSkillDto
                {
                    SkillId = skillId,
                    SkillType = SkillType.Predefined,
                    CustomSkillName = null
                }
            ]
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess, result.IsError ? result.TopError.Code + " - " + result.TopError.Description : "");
        
        var skillRepo = _fixture.Services.GetRequiredService<InMemorySkillRepository>();
        Assert.Single(skillRepo.FreelancerSkills);
        var freelancerSkill = skillRepo.FreelancerSkills.First();
        Assert.Equal(profileId, freelancerSkill.FreelancerId);
        Assert.Equal(skillId, freelancerSkill.SkillId);
        Assert.Equal(SkillType.Predefined, freelancerSkill.SkillType);
    }

    [Fact]
    public async Task AddSkillsToFreelancer_CustomSkill_ShouldSucceed()
    {
        // Arrange
        var (userId, profileId) = SeedActivatedFreelancer();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new AddSkillsToFreelacnerCommand(
            Skills: [
                new CreateFreelancerSkillDto
                {
                    SkillId = null,
                    SkillType = SkillType.Custom,
                    CustomSkillName = "Blazor WebAssembly"
                }
            ]
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        
        var skillRepo = _fixture.Services.GetRequiredService<InMemorySkillRepository>();
        Assert.Single(skillRepo.FreelancerSkills);
        var freelancerSkill = skillRepo.FreelancerSkills.First();
        Assert.Equal(profileId, freelancerSkill.FreelancerId);
        Assert.Equal("Blazor WebAssembly", freelancerSkill.CustomSkillName);
        Assert.Equal(SkillType.Custom, freelancerSkill.SkillType);
    }

    [Fact]
    public async Task AddSkillsToFreelancer_PredefinedSkillNotExists_ShouldFail()
    {
        // Arrange
        var (userId, _) = SeedActivatedFreelancer();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new AddSkillsToFreelacnerCommand(
            Skills: [
                new CreateFreelancerSkillDto
                {
                    SkillId = Guid.NewGuid(), // non-existent
                    SkillType = SkillType.Predefined,
                    CustomSkillName = null
                }
            ]
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    [Fact]
    public async Task AddSkillsToFreelancer_WithoutAuthentication_ShouldFail()
    {
        // Arrange
        await using var scope = _fixture.Services.CreateAsyncScope();

        var command = new AddSkillsToFreelacnerCommand(
            Skills: [
                new CreateFreelancerSkillDto
                {
                    SkillId = null,
                    SkillType = SkillType.Custom,
                    CustomSkillName = "Blazor"
                }
            ]
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User.NotAuthenticated", result.TopError.Code);
    }
    
    [Fact]
    public async Task AddSkillsToFreelancer_PredefinedWithMissingSkillId_ShouldFailValidation()
    {
        // Arrange
        var (userId, _) = SeedActivatedFreelancer();

        await using var scope = _fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<FakeUser>().SetUser(userId);

        var command = new AddSkillsToFreelacnerCommand(
            Skills: [
                new CreateFreelancerSkillDto
                {
                    SkillId = null, // Missing ID for Predefined type
                    SkillType = SkillType.Predefined,
                    CustomSkillName = null
                }
            ]
        );

        // Act
        var result = await scope.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }
}
