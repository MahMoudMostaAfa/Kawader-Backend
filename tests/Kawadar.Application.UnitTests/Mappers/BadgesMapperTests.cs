using AutoMapper;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Application.Features.Badges.Mapper;
using Kawadar.Domain.Badges;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class BadgesMapperTests
{
  private readonly IMapper _mapper;

  public BadgesMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<BadgeMapper>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapBadgeDto_ValidBadge_MapsEveryField()
  {
    // Arrange
    var badgeResult = Badge.Create("Top Rated", "https://cdn/badge.png", "Awarded for consistent quality");
    Assert.True(badgeResult.IsSuccess);
    var badge = badgeResult.Value;

    // Act
    var result = _mapper.Map<BadgeDTO>(badge);

    // Assert
    Assert.Equal(badge.Id, result.Id);
    Assert.Equal(badge.Title, result.Title);
    Assert.Equal(badge.IconUrl, result.IconUrl);
    Assert.Equal(badge.Description, result.Description);
  }

  [Fact]
  public void MapBadgeDto_NullSource_ReturnsNull()
  {
    // Arrange
    Badge? badge = null;

    // Act
    var result = _mapper.Map<BadgeDTO?>(badge);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void MapBadgeDto_LongStrings_MapsWithoutTruncation()
  {
    // Arrange
    var title = new string('A', 200);
    var icon = "https://cdn/" + new string('b', 128) + ".png";
    var description = new string('D', 500);

    var badgeResult = Badge.Create(title, icon, description);
    Assert.True(badgeResult.IsSuccess);

    // Act
    var result = _mapper.Map<BadgeDTO>(badgeResult.Value);

    // Assert
    Assert.Equal(title, result.Title);
    Assert.Equal(icon, result.IconUrl);
    Assert.Equal(description, result.Description);
  }
}
