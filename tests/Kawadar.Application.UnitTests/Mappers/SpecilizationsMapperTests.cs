using AutoMapper;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Application.Features.Specilizations.Mapper;
using Kawadar.Domain.Specilizations;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class SpecilizationsMapperTests
{
  private readonly IMapper _mapper;

  public SpecilizationsMapperTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<SpecilizationMapper>(), NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void MapSpecilizationDto_ValidSource_MapsEveryField()
  {
    // Arrange
    var specResult = Specilization.Create("Backend", true);
    Assert.True(specResult.IsSuccess);
    var spec = specResult.Value;

    // Act
    var result = _mapper.Map<SpecilizationDTO>(spec);

    // Assert
    Assert.Equal(spec.Id, result.Id);
    Assert.Equal(spec.Name, result.Name);
    Assert.Equal(spec.IsActive, result.IsActive);
  }

  [Fact]
  public void MapSpecilizationDto_NullSource_ReturnsNull()
  {
    // Arrange
    Specilization? spec = null;

    // Act
    var result = _mapper.Map<SpecilizationDTO?>(spec);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void MapSpecilizationDto_InactiveSource_MapsIsActiveAsFalse()
  {
    // Arrange
    var specResult = Specilization.Create("Legacy", false);
    Assert.True(specResult.IsSuccess);

    // Act
    var result = _mapper.Map<SpecilizationDTO>(specResult.Value);

    // Assert
    Assert.False(result.IsActive);
  }
}
