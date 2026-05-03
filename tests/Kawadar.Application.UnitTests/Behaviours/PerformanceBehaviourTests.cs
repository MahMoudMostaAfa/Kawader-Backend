using Kawadar.Application.Common.Behaviours;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kawadar.Application.UnitTests.Behaviours;

public class PerformanceBehaviourTests
{
  private readonly ILogger<PerformanceTestRequest> _logger;
  private readonly IUser _user;
  private readonly TimeProvider _timeProvider;
  private readonly PerformanceBehaviour<PerformanceTestRequest, Result<Success>> _sut;

  public PerformanceBehaviourTests()
  {
    _logger = Substitute.For<ILogger<PerformanceTestRequest>>();
    _user = Substitute.For<IUser>();
    _timeProvider = Substitute.For<TimeProvider>();
    _sut = new PerformanceBehaviour<PerformanceTestRequest, Result<Success>>(_logger, _user, _timeProvider);
  }

  [Fact]
  public async Task Handle_ExecutionIsFast_ContinuesPipelineWithoutWarningLog()
  {
    // Arrange
    var request = new PerformanceTestRequest("fast");
    _user.Id.Returns("user-1");
    _timeProvider.TimestampFrequency.Returns(1000);
    _timeProvider.GetTimestamp().Returns(0, 100); // 100 ticks is very fast

    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.DidNotReceive().Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Warning),
      Arg.Any<EventId>(),
      Arg.Any<object>(),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Handle_ExecutionExceedsThreshold_LogsWarningAndContinuesPipeline()
  {
    // Arrange
    var request = new PerformanceTestRequest("slow");
    _user.Id.Returns("user-2");
    
    // Set frequency to 1000 so 1 tick = 1ms
    _timeProvider.TimestampFrequency.Returns(1000);
    _timeProvider.GetTimestamp().Returns(0, 600); // 0 at start, 600 at end -> 600ms

    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Warning),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => state.ToString()!.Contains("Long Running Request")),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Handle_UserIdIsNullAndExecutionExceedsThreshold_LogsWarningWithEmptyUserId()
  {
    // Arrange
    var request = new PerformanceTestRequest("slow-no-user");
    _user.Id.Returns((string?)null);
    
    _timeProvider.TimestampFrequency.Returns(1000);
    _timeProvider.GetTimestamp().Returns(0, 600);

    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Warning),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => 
        state.ToString()!.Contains(nameof(PerformanceTestRequest)) &&
        state.ToString()!.Contains("  ")), // Check for double space indicating empty UserId/UserName
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  public sealed record PerformanceTestRequest(string Name) : IRequest<Result<Success>>;
}
