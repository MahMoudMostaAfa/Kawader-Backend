using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Application.Features.Proposals.Mappers;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.QuestionAnswers;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Kawadar.Tests.Common.Proposals;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class ProposalsMapperTests
{
  private readonly IMapper _mapper;

  public ProposalsMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<ProposalDetailsProfile>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapProposalDetailsDto_ValidTuple_MapsEveryFieldIncludingCollections()
  {
    // Arrange
    var proposal = JobProposalFactory.CreateValidMilestoneBased();

    var milestone = ProposalMilestoneFactory.CreateValid();
    proposal.AddMilestone(milestone);

    var questionResult = JobQuestion.Create("Can you deliver this in one week?", true, 1);
    Assert.True(questionResult.IsSuccess);
    var question = questionResult.Value;

    var answer = ProposalQuestionAnswerFactory.CreateValid();
    SetPrivateProperty(answer, nameof(ProposalQuestionAnswer.Question), question);
    proposal.AddQuestionAnswer(answer);

    var userProfileResult = UserProfile.create("u-id", "Nour", "Hany", ProfileType.Freelancer);
    Assert.True(userProfileResult.IsSuccess);
    var userProfile = userProfileResult.Value;
    userProfile.UpdateProfilePicture("https://cdn/nour.png");

    var user = new UserDto
    {
      Id = "u-id",
      Email = "nour@kawadar.dev",
      UserName = "nour_hany",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<ProposalDetailsDto>((proposal, user, userProfile));

    // Assert
    Assert.Equal(proposal.JobId, result.JobId);
    Assert.Equal(proposal.CoverLetter, result.CoverLetter);
    Assert.Equal(proposal.ProposalType, result.JobProposalType);
    Assert.Equal(user.UserName, result.ProposalByUserName);
    Assert.Equal(userProfile.FullName, result.ProposalByFullName);
    Assert.Equal(userProfile.ProfilePictureUrl, result.ProposalByPhoto);
    Assert.Equal(proposal.Amount, result.Amount);
    Assert.Equal(proposal.EstimatedDays, result.EstimatedDays);
    Assert.Equal(proposal.HourlyRate, result.HourlyRate);
    Assert.Equal(proposal.EstimatedHours, result.EstimatedHours);

    Assert.NotNull(result.Milestones);
    Assert.Single(result.Milestones!);
    Assert.Equal(milestone.Title, result.Milestones[0].Title);
    Assert.Equal(milestone.Description, result.Milestones[0].Description);
    Assert.Equal(milestone.Amount, result.Milestones[0].Amount);
    Assert.Equal(milestone.DueDate, result.Milestones[0].DueDate);

    Assert.NotNull(result.QuestionsWithAnswer);
    Assert.Single(result.QuestionsWithAnswer!);
    Assert.Equal(question.Question, result.QuestionsWithAnswer[0].Question);
    Assert.Equal(answer.Answer, result.QuestionsWithAnswer[0].Answer);
  }

  [Fact]
  public void MapProposalDetailsDto_QuestionNavigationIsNull_MapsQuestionAsNull()
  {
    // Arrange
    var proposal = JobProposalFactory.CreateValidMilestoneBased();
    var answer = ProposalQuestionAnswerFactory.CreateValid();
    proposal.AddQuestionAnswer(answer);

    var userProfileResult = UserProfile.create("u-id", "Nour", "Hany", ProfileType.Freelancer);
    Assert.True(userProfileResult.IsSuccess);

    var user = new UserDto
    {
      Id = "u-id",
      Email = "nour@kawadar.dev",
      UserName = "nour_hany",
    };

    // Act
    var result = _mapper.Map<ProposalDetailsDto>((proposal, user, userProfileResult.Value));

    // Assert
    Assert.NotNull(result.QuestionsWithAnswer);
    Assert.Single(result.QuestionsWithAnswer!);
    Assert.Null(result.QuestionsWithAnswer[0].Question);
    Assert.Equal(answer.Answer, result.QuestionsWithAnswer[0].Answer);
  }

  [Fact]
  public void MapProposalDetailsDto_NullUser_MapsProposalByUserNameAsNull()
  {
    // Arrange
    var proposal = JobProposalFactory.CreateValidOneTime();
    var userProfileResult = UserProfile.create("u-id", "Nour", "Hany", ProfileType.Freelancer);
    Assert.True(userProfileResult.IsSuccess);

    // Act
    var result = _mapper.Map<ProposalDetailsDto>((proposal, (UserDto)null!, userProfileResult.Value));

    // Assert
    Assert.Equal(proposal.JobId, result.JobId);
    Assert.Equal(proposal.CoverLetter, result.CoverLetter);
    Assert.Equal(userProfileResult.Value.FullName, result.ProposalByFullName);
    Assert.Null(result.ProposalByUserName);
  }

  private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
  {
    var property = typeof(TTarget).GetProperty(propertyName);
    Assert.NotNull(property);
    property!.SetValue(target, value);
  }
}
