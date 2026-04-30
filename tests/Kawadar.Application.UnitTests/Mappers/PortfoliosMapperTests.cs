using AutoMapper;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Items.Enum;
using Kawadar.Domain.Portfolios.Project;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class PortfoliosMapperTests
{
  private readonly IMapper _mapper;

  public PortfoliosMapperTests()
  {
    var config = new MapperConfiguration(cfg =>
    {
      cfg.AddProfile<ProjectMapper>();
      cfg.AddProfile<ItemMapper>();
    }, NullLoggerFactory.Instance);

    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapProjectDto_ValidProject_MapsEveryField()
  {
    // Arrange
    var specilizationId = Guid.NewGuid();
    var projectResult = PortfolioProject.Create(
      "Kawadar Platform",
      "Freelance marketplace platform",
      specilizationId,
      Guid.NewGuid(),
      "https://cdn/projects/p1.png",
      3,
      "https://kawadar.dev/project");

    Assert.True(projectResult.IsSuccess);
    var project = projectResult.Value;

    // Act
    var result = _mapper.Map<ProjectDTO>(project);

    // Assert
    Assert.Equal(project.Id, result.Id);
    Assert.Equal(project.Title, result.title);
    Assert.Equal(project.Description, result.description);
    Assert.Equal(project.ProjectImageUrl, result.ProjectImageUrl);
    Assert.Equal(project.ProjectUrl, result.ProjectUrl);
    Assert.Equal(project.DisplayOrder, result.displayOrder);
  }

  [Fact]
  public void MapItemDto_ValidItem_MapsEveryField()
  {
    // Arrange
    var itemResult = PortfolioItem.Create(ItemType.Link, "https://github.com/kawadar", 2, Guid.NewGuid());
    Assert.True(itemResult.IsSuccess);
    var item = itemResult.Value;

    // Act
    var result = _mapper.Map<ItemDTO>(item);

    // Assert
    Assert.Equal(item.Id, result.Id);
    Assert.Equal(item.ItemType, result.itemType);
    Assert.Equal(item.Content, result.content);
    Assert.Equal(item.DisplayOrder, result.displayOrder);
  }

  [Fact]
  public void MapProjectDto_NullSource_ReturnsNull()
  {
    // Arrange
    PortfolioProject? project = null;

    // Act
    var result = _mapper.Map<ProjectDTO?>(project);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void MapItemDto_NullSource_ReturnsNull()
  {
    // Arrange
    PortfolioItem? item = null;

    // Act
    var result = _mapper.Map<ItemDTO?>(item);

    // Assert
    Assert.Null(result);
  }
}
