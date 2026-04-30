using AutoMapper;
using Xunit;

namespace Kawadar.Application.UnitTests.Mappers;

public class AuthMapperTests
{
  private readonly Type[] _profileTypes;

  public AuthMapperTests()
  {
    _profileTypes = typeof(Kawadar.Application.DependencyInjection).Assembly
      .GetTypes()
      .Where(x => typeof(Profile).IsAssignableFrom(x) && x.Namespace != null && x.Namespace.Contains("Features.Auth"))
      .ToArray();
  }

  [Fact]
  public void AuthFeature_NoMapperProfiles_ExpectedResult()
  {
    // Arrange
    // Act
    // Assert
    Assert.Empty(_profileTypes);
  }
}
