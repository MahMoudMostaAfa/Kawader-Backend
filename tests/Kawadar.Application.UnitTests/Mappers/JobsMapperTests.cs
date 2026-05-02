using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Application.Features.Jobs.Mappers;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobReports.Enums;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Kawadar.Tests.Common.Jobs;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class JobsMapperTests
{
  private readonly IMapper _mapper;

  public JobsMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<JobDetailsProfile>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapJobDetailsDto_ValidTuple_MapsEveryFieldAndNestedCollections()
  {
    // Arrange
    var question = JobFactory.CreateQuestion("What is your approach?", true, 1);
    var skill = JobFactory.CreateSkill("C#");
    var attachment = JobFactory.CreateAttachment("brief.pdf", "https://cdn/jobs/brief.pdf", 321, "application/pdf");

    var job = JobFactory.Builder()
      .WithTitle("Senior Backend Developer")
      .WithDescription("Build and maintain APIs")
      .WithSlug("Senior Backend/Developer")
      .WithJobType(JobType.Hourly)
      .WithBudgetRange(BudgetRange.From5000To10000)
      .WithHourlyRateRange(HourlyRateRange.From200To500)
      .WithDurationInDays(30)
      .WithExperienceLevel(JobExperienceLevel.SeniorLevel)
      .WithQuestions([question])
      .WithSkills([skill])
      .WithAttachments([attachment])
      .Build();

    var specResult = Specilization.Create("Backend", true);
    Assert.True(specResult.IsSuccess);
    SetPrivateProperty(job, nameof(Job.Specilization), specResult.Value);

    var profile = CreateUserProfile("Mona", "Samir", ProfileType.Client);
    profile.UpdateProfilePicture("https://cdn/users/mona.png");

    var userDto = new UserDto
    {
      Id = "u-1",
      Email = "mona@kawadar.dev",
      UserName = "mona_client",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<JobDetailsDto>((job, userDto, profile));

    // Assert
    Assert.Equal(job.Title, result.Title);
    Assert.Equal(job.Description, result.Description);
    Assert.Equal(profile.FullName, result.PosterFullName);
    Assert.Equal(profile.ProfilePictureUrl, result.PosterProfilePictureUrl);
    Assert.Equal(userDto.UserName, result.PosterUsername);
    Assert.Equal(Uri.EscapeDataString(job.JobSlug), result.JobSlug);
    Assert.Equal(job.Specilization.Name, result.Specilization);
    Assert.Equal(job.JobType, result.JobType);
    Assert.Equal(job.BudgetRange, result.BudgetRange);
    Assert.Equal(job.HourlyRateRange, result.HourlyRateRange);
    Assert.Equal(job.DurationInDays, result.DurationInDays);
    Assert.Equal(job.ExperienceLevel, result.ExperienceLevel);
    Assert.Equal(job.JobStatus, result.JobStatus);

    Assert.Single(result.Questions);
    Assert.Equal(question.Id, result.Questions[0].Id);
    Assert.Equal(question.Question, result.Questions[0].QuestionText);
    Assert.Equal(question.IsRequired, result.Questions[0].IsRequired);
    Assert.Equal(question.DisplayOrder, result.Questions[0].DisplayOrder);

    Assert.Single(result.Skills);
    Assert.Equal(skill.Id, result.Skills[0].Id);
    Assert.Equal(skill.Name, result.Skills[0].SkillName);

    Assert.Single(result.Attachments);
    Assert.Equal(attachment.Id, result.Attachments[0].Id);
    Assert.Equal(attachment.File.FileName, result.Attachments[0].FileName);
    Assert.Equal(attachment.File.FileUrl, result.Attachments[0].FileUrl);
    Assert.Equal(attachment.File.MimeType, result.Attachments[0].ContentType);
    Assert.Equal(attachment.File.FileSizeInBytes, result.Attachments[0].FileSizeInBytes);
  }

  [Fact]
  public void MapJobSummaryDto_ValidJob_MapsEveryField()
  {
    // Arrange
    var skill = JobFactory.CreateSkill(".NET");

    var job = JobFactory.Builder()
      .WithTitle("API Developer")
      .WithDescription("Build APIs")
      .WithSlug("API Developer")
      .WithJobType(JobType.FixedPrice)
      .WithBudgetRange(BudgetRange.From1000To5000)
      .WithHourlyRateRange(HourlyRateRange.From100To200)
      .WithDurationInDays(14)
      .WithExperienceLevel(JobExperienceLevel.MidLevel)
      .WithSkills([skill])
      .WithoutQuestions()
      .WithoutAttachments()
      .Build();

    var specResult = Specilization.Create("Software", true);
    Assert.True(specResult.IsSuccess);
    SetPrivateProperty(job, nameof(Job.Specilization), specResult.Value);
    job.CreatedAt = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc);

    // Act
    var result = _mapper.Map<JobSummaryDto>(job);

    // Assert
    Assert.Equal(job.Title, result.Title);
    Assert.Equal(job.Description, result.Description);
    Assert.Equal(Uri.EscapeDataString(job.JobSlug), result.JobSlug);
    Assert.Equal(job.Specilization.Name, result.Specilization);
    Assert.Equal(job.JobType, result.JobType);
    Assert.Equal(job.BudgetRange, result.BudgetRange);
    Assert.Equal(job.HourlyRateRange, result.HourlyRateRange);
    Assert.Equal(job.DurationInDays, result.DurationInDays);
    Assert.Equal(job.ExperienceLevel, result.ExperienceLevel);
    Assert.Equal(job.JobStatus, result.JobStatus);
    Assert.Equal(job.CreatedAt, result.CreatedAt);

    Assert.Single(result.Skills);
    Assert.Equal(skill.Id, result.Skills[0].Id);
    Assert.Equal(skill.Name, result.Skills[0].SkillName);
  }

  [Fact]
  public void MapFullJobReportDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var job = JobFactory.CreateValid();
    var reportResult = JobReport.Create(job.Id, Guid.NewGuid(), "Scam report", ReportType.Scam);
    Assert.True(reportResult.IsSuccess);

    var report = reportResult.Value;
    report.Update(ReportStatus.Resolved, "Blocked account");

    var userDto = new UserDto
    {
      Id = "u-9",
      UserName = "reporter_user",
      Email = "reporter@kawadar.dev",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<FullJobReportDto>((report, job, userDto));

    // Assert
    Assert.Equal(report.Id, result.Id);
    Assert.Equal(job.JobSlug, result.JobSlug);
    Assert.Equal(userDto.UserName, result.UserName);
    Assert.Equal(report.ReportStatus, result.ReportStatus);
    Assert.Equal(report.ReportType, result.ReportType);
    Assert.Equal(report.Content, result.Content);
    Assert.Equal(report.ActionTaken, result.ActionTaken);
  }

  [Fact]
  public void MapBriefJobReportDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var job = JobFactory.Builder().WithTitle("Moderation Job").Build();
    var reportResult = JobReport.Create(job.Id, Guid.NewGuid(), "Abuse", ReportType.Harassement);
    Assert.True(reportResult.IsSuccess);

    var report = reportResult.Value;
    report.Update(ReportStatus.InReview, "Under review");

    var userDto = new UserDto
    {
      Id = "u-3",
      UserName = "mod_reviewer",
      Email = "mod@kawadar.dev",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<BriefJobReportDto>((report, job, userDto));

    // Assert
    Assert.Equal(report.Id, result.Id);
    Assert.Equal(job.Title, result.JobTitle);
    Assert.Equal(userDto.UserName, result.ReporterUserName);
    Assert.Equal(report.ReportStatus, result.ReportStatus);
    Assert.Equal(report.ReportType, result.ReportType);
  }

  [Fact]
  public void MapJobDetailsDto_NullSpecilization_MapsSpecilizationAsNull()
  {
    // Arrange
    var job = JobFactory.CreateValid();
    var userDto = new UserDto { Id = "u-5", UserName = "x", Email = "x@k.dev" };
    var profile = CreateUserProfile("A", "B", ProfileType.Client);

    // Act
    var result = _mapper.Map<JobDetailsDto>((job, userDto, profile));

    // Assert
    Assert.Equal(job.Title, result.Title);
    Assert.Equal(job.Description, result.Description);
    Assert.Null(result.Specilization);
  }

  private static UserProfile CreateUserProfile(string firstName, string lastName, ProfileType profileType)
  {
    var result = UserProfile.create(Guid.NewGuid().ToString(), firstName, lastName, profileType);
    Assert.True(result.IsSuccess);
    return result.Value;
  }

  private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
  {
    var property = typeof(TTarget).GetProperty(propertyName);
    Assert.NotNull(property);
    property!.SetValue(target, value);
  }
}
