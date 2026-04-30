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
  private readonly PerformanceBehaviour<PerformanceTestRequest, Result<Success>> _sut;

  public PerformanceBehaviourTests()
  {
    _logger = Substitute.For<ILogger<PerformanceTestRequest>>();
    _user = Substitute.For<IUser>();
    _sut = new PerformanceBehaviour<PerformanceTestRequest, Result<Success>>(_logger, _user);
  }

  [Fact]
  public async Task Handle_ExecutionIsFast_ContinuesPipelineWithoutWarningLog()
  {
    // Arrange
    var request = new PerformanceTestRequest("fast");
    _user.Id.Returns("user-1");

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

    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next
      .Invoke(Arg.Any<CancellationToken>())
      .Returns(_ => DelayedSuccessAsync());

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

    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next
      .Invoke(Arg.Any<CancellationToken>())
      .Returns(_ => DelayedSuccessAsync());

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Warning),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => state.ToString()!.Contains(nameof(PerformanceTestRequest))),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  public sealed record PerformanceTestRequest(string Name) : IRequest<Result<Success>>;

  private static async Task<Result<Success>> DelayedSuccessAsync()
  {
    await Task.Delay(550);
    return Result.Success;
  }
}
