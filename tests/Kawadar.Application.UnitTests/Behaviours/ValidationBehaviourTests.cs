using FluentValidation;
using FluentValidation.Results;
using Kawadar.Application.Common.Behaviours;
using Kawadar.Domain.Common.Results;
using MediatR;
using NSubstitute;
using Xunit;

namespace Kawadar.Application.UnitTests.Behaviours;

public class ValidationBehaviourTests
{
  private readonly IValidator<ValidationTestRequest> _validator;
  private readonly ValidationBehaviour<ValidationTestRequest, Result<Success>> _sut;

  public ValidationBehaviourTests()
  {
    _validator = Substitute.For<IValidator<ValidationTestRequest>>();
    _sut = new ValidationBehaviour<ValidationTestRequest, Result<Success>>(_validator);
  }

  [Fact]
  public async Task Handle_ValidatorIsNull_CallsNextOnceAndReturnsNextResult()
  {
    // Arrange
    var sut = new ValidationBehaviour<ValidationTestRequest, Result<Success>>();
    var request = new ValidationTestRequest("value");
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ValidationPasses_CallsValidatorThenContinuesPipeline()
  {
    // Arrange
    var request = new ValidationTestRequest("valid");
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    _validator
      .ValidateAsync(Arg.Is<ValidationTestRequest>(x => x.Name == "valid"), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await _validator.Received(1)
      .ValidateAsync(Arg.Is<ValidationTestRequest>(x => x.Name == "valid"), Arg.Any<CancellationToken>());
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ValidationFails_ReturnsValidationErrorsAndStopsPipeline()
  {
    // Arrange
    var request = new ValidationTestRequest("invalid");
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();

    var failures = new List<ValidationFailure>
    {
      new("Name", "Name is required")
    };

    _validator
      .ValidateAsync(Arg.Is<ValidationTestRequest>(x => x.Name == "invalid"), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult(failures));

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsError);
    Assert.Single(result.Errors);
    Assert.Equal("Name", result.Errors[0].Code);
    Assert.Equal("Name is required", result.Errors[0].Description);

    await _validator.Received(1)
      .ValidateAsync(Arg.Is<ValidationTestRequest>(x => x.Name == "invalid"), Arg.Any<CancellationToken>());
    await next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_RequestIsNullAndValidatorIsNull_ContinuesPipeline()
  {
    // Arrange
    var sut = new ValidationBehaviour<ValidationTestRequest, Result<Success>>();
    ValidationTestRequest request = null!;
    var next = Substitute.For<RequestHandlerDelegate<Result<Success>>>();
    next.Invoke(Arg.Any<CancellationToken>()).Returns(Result.Success);

    // Act
    var result = await sut.Handle(request, next, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    await next.Received(1).Invoke(Arg.Any<CancellationToken>());
  }

  public sealed record ValidationTestRequest(string Name) : IRequest<Result<Success>>;
}
