using kawadar.Application.SubcutaneousTests.Common.Fixtures;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace kawadar.Application.SubcutaneousTests;

public class InfrastructureTests : IClassFixture<SubcutaneousTestFixture>
{
    private readonly SubcutaneousTestFixture _fixture;

    public InfrastructureTests(SubcutaneousTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Fixture_ShouldResolveMediator()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();

        // Act
        var mediator = scope.ServiceProvider.GetService<IMediator>();

        // Assert
        Assert.NotNull(mediator);
    }
}
