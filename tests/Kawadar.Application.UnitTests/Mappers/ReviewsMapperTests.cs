using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Application.Features.Reviews.Mapper;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Reviews.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class ReviewsMapperTests
{
  private readonly IMapper _mapper;

  public ReviewsMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMapper>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapReviewDto_ValidTuple_MapsEveryField()
  {
    // Arrange
    var reviewResult = Review.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ReviewType.FreelancerClient, 4.5f, "Great communication");
    Assert.True(reviewResult.IsSuccess);

    var user = new UserDto
    {
      Id = "identity-reviewer",
      UserName = "reviewer_1",
      Email = "reviewer@kawadar.dev",
      EmailConfirmed = true,
    };

    // Act
    var result = _mapper.Map<ReviewDto>((reviewResult.Value, user));

    // Assert
    Assert.Equal(reviewResult.Value.JobId, result.JobId);
    Assert.Equal(user.UserName, result.ReviewerUserName);
    Assert.Equal(reviewResult.Value.Rating, result.Rating);
    Assert.Equal(reviewResult.Value.Comment, result.Comment);
  }

  [Fact]
  public void MapReviewDto_NullUser_MapsReviewerUserNameAsNull()
  {
    // Arrange
    var reviewResult = Review.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ReviewType.ClientFreelancer, 5f, "Excellent");
    Assert.True(reviewResult.IsSuccess);

    // Act
    var result = _mapper.Map<ReviewDto>((reviewResult.Value, (UserDto)null!));

    // Assert
    Assert.Equal(reviewResult.Value.JobId, result.JobId);
    Assert.Equal(reviewResult.Value.Rating, result.Rating);
    Assert.Equal(reviewResult.Value.Comment, result.Comment);
    Assert.Null(result.ReviewerUserName);
  }

  [Fact]
  public void MapReviewDto_EmptyComment_MapsEdgeValue()
  {
    // Arrange
    var reviewResult = Review.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ReviewType.ClientFreelancer, 0f, string.Empty);
    Assert.True(reviewResult.IsSuccess);

    var user = new UserDto
    {
      Id = "identity-reviewer",
      UserName = "reviewer_2",
      Email = "reviewer2@kawadar.dev",
    };

    // Act
    var result = _mapper.Map<ReviewDto>((reviewResult.Value, user));

    // Assert
    Assert.Equal(0f, result.Rating);
    Assert.Equal(string.Empty, result.Comment);
    Assert.Equal(user.UserName, result.ReviewerUserName);
  }
}
