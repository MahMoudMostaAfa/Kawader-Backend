using AutoMapper;
using FluentValidation;
using Kawadar.Application.Common.Behaviours;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Auth.Commands.Login;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Kawadar.Application.UnitTests;

public class ApplicationWiringTests
{
  [Fact]
  public void AddApplication_BuildServiceProvider_ResolvesAndOrdersCoreComponents()
  {
    // Arrange
    var services = new ServiceCollection();
    
    // Add required external dependencies
    services.AddSingleton(Substitute.For<IUser>());
    services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
    services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
    
    services.AddSingleton(Substitute.For<IIdentityService>());
    services.AddSingleton(Substitute.For<ITokenProvider>());
    services.AddSingleton(Substitute.For<IUsersRepository>());
    services.AddSingleton(Substitute.For<IUnitOfWork>());

    // Act
    services.AddApplication();
    var serviceProvider = services.BuildServiceProvider();

    // Assert
    
    // 1. Prove Handler Activation
    var handler = serviceProvider.GetService<IRequestHandler<LoginCommand, Result<LoginDto>>>();
    Assert.NotNull(handler);

    // 2. Prove Validator Activation
    var validator = serviceProvider.GetService<IValidator<LoginCommand>>();
    Assert.NotNull(validator);

    // 3. Prove Pipeline Behaviours Activation and Order
    var behaviors = serviceProvider.GetServices<IPipelineBehavior<LoginCommand, Result<LoginDto>>>().ToList();

    // According to DependencyInjection.cs and MediatR defaults:
    // MediatR adds RequestPreProcessorBehavior at the start when pre-processors are used.
    Assert.Contains(behaviors, b => b.GetType().Name.StartsWith("RequestPreProcessorBehavior"));
    Assert.Contains(behaviors, b => b is ValidationBehaviour<LoginCommand, Result<LoginDto>>);
    Assert.Contains(behaviors, b => b is UnHandledExceptionBehaviour<LoginCommand, Result<LoginDto>>);
    Assert.Contains(behaviors, b => b is PerformanceBehaviour<LoginCommand, Result<LoginDto>>);

    // Verify relative order of our custom behaviors
    var customBehaviors = behaviors.Where(b => !b.GetType().Name.StartsWith("RequestPreProcessorBehavior")).ToList();
    Assert.Collection(customBehaviors,
      b => Assert.IsType<ValidationBehaviour<LoginCommand, Result<LoginDto>>>(b),
      b => Assert.IsType<UnHandledExceptionBehaviour<LoginCommand, Result<LoginDto>>>(b),
      b => Assert.IsType<PerformanceBehaviour<LoginCommand, Result<LoginDto>>>(b)
    );

    // 4. Prove PreProcessor Activation
    var preProcessors = serviceProvider.GetServices<IRequestPreProcessor<LoginCommand>>().ToList();
    Assert.Contains(preProcessors, p => p is LoggingBehaviour<LoginCommand>);

    // 5. Prove AutoMapper Configuration Validity
    var mapperConfig = serviceProvider.GetService<IConfigurationProvider>();
    Assert.NotNull(mapperConfig);
    var mapperConfiguration = mapperConfig as MapperConfiguration;
    Assert.NotNull(mapperConfiguration);
    mapperConfiguration.AssertConfigurationIsValid();
  }
}