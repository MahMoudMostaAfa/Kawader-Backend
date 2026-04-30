using AutoMapper;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class SkillsMapperTests
{
  private readonly Type[] _profileTypes;

  public SkillsMapperTests()
  {
    _profileTypes = typeof(Kawadar.Application.DependencyInjection).Assembly
      .GetTypes()
      .Where(x => typeof(Profile).IsAssignableFrom(x) && x.Namespace != null && x.Namespace.Contains("Features.Skills"))
      .ToArray();
  }

  [Fact]
  public void SkillsFeature_NoMapperProfiles_ExpectedResult()
  {
    // Arrange
    // Act
    // Assert
    Assert.Empty(_profileTypes);
  }
}
