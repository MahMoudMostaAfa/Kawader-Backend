using Kawadar.Application.Common.Behaviours;
using Kawadar.Application.Common.Interfaces.Auth;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kawadar.Application.UnitTests.Behaviours;

public class LoggingBehaviourTests
{
  private readonly ILogger<LoggingTestRequest> _logger;
  private readonly IUser _user;
  private readonly LoggingBehaviour<LoggingTestRequest> _sut;

  public LoggingBehaviourTests()
  {
    _logger = Substitute.For<ILogger<LoggingTestRequest>>();
    _user = Substitute.For<IUser>();
    _sut = new LoggingBehaviour<LoggingTestRequest>(_logger, _user);
  }

  [Fact]
  public async Task Process_ValidRequest_LogsInformationOnce()
  {
    // Arrange
    var request = new LoggingTestRequest("normal");
    _user.Id.Returns("user-123");

    // Act
    await _sut.Process(request, CancellationToken.None);

    // Assert
    var _ = _user.Received(1).Id;

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Information),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => state.ToString()!.Contains("Request:")),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Process_UserIdIsNull_StillLogsInformation()
  {
    // Arrange
    var request = new LoggingTestRequest("edge");
    _user.Id.Returns((string?)null);

    // Act
    await _sut.Process(request, CancellationToken.None);

    // Assert
    var _ = _user.Received(1).Id;

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Information),
      Arg.Any<EventId>(),
      Arg.Is<object>(state => 
        state.ToString()!.Contains(nameof(LoggingTestRequest)) && 
        state.ToString()!.Contains("  ")), // Check for double space indicating empty UserId/UserName
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task Process_RequestIsNull_LogsInformationWithoutThrowing()
  {
    // Arrange
    LoggingTestRequest request = null!;
    _user.Id.Returns("user-123");

    // Act
    await _sut.Process(request, CancellationToken.None);

    // Assert
    var _ = _user.Received(1).Id;

    _logger.Received(1).Log(
      Arg.Is<LogLevel>(x => x == LogLevel.Information),
      Arg.Any<EventId>(),
      Arg.Any<object>(),
      Arg.Any<Exception>(),
      Arg.Any<Func<object, Exception?, string>>());
  }

  public sealed record LoggingTestRequest(string Value);
}
