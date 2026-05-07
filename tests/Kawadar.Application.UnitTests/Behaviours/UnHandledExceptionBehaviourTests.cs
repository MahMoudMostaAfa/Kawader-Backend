using Kawadar.Application.Common.Behaviours;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kawadar.Application.UnitTests.Behaviours;

public class UnHandledExceptionBehaviourTests
{
  private readonly ILogger<UnhandledTestRequest> _logger;
  private readonly UnHandledExceptionBehaviour<UnhandledTestRequest, Result<Success>> _sut;

  public UnHandledExceptionBehaviourTests()
  {
    _logger = Substitute.For<ILogger<UnhandledTestRequest>>();
    _sut = new UnHandledExceptionBehaviour<UnhandledTestRequest, Result<Success>>(_logger);
  }

  [Fact]
  public async Task Handle_NextSucceeds_ReturnsResponseAndDoesNotLogError()
  {
    // Arrange
    var request = new UnhandledTestRequest("ok");
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.DidNotReceive().Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Error),
      Arg.Any<EventId>(),
      Arg.Any<object>(),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Handle_NextThrows_LogsErrorAndRethrows()
  {
    // Arrange
    var request = new UnhandledTestRequest("boom");
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    var expectedException = new InvalidOperationException("failure");

    next
      .Invoke(Arg.Any<CancellationToken>())
      .Returns(_ => Task.FromException<Result<Success>>(expectedException));

    // Act
    var action = () => _sut.Handle(request, next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
    Assert.Equal("failure", exception.Message);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Error),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => state.ToString()!.Contains("Unhandled exception for request")),
      Arg.Is<Exception>(ex => ex.Message == "failure"),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Handle_RequestIsNullAndNextSucceeds_ReturnsResponseWithoutErrorLog()
  {
    // Arrange
    UnhandledTestRequest request = null!;
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());

    _logger.DidNotReceive().Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Error),
      Arg.Any<EventId>(),
      Arg.Any<object>(),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  public sealed record UnhandledTestRequest(string Value) : IRequest<Result<Success>>;
}
