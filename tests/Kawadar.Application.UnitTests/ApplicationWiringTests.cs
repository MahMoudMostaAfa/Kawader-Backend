using System.Reflection;
using AutoMapper;
using FluentValidation;
using Kawadar.Application;
using Kawadar.Application.Common.Behaviours;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kawadar.Application.UnitTests;

public class ApplicationWiringTests
{
  [Fact]
  public void AddApplication_AllRequestHandlersAreRegistered()
  {
    // Arrange
    var services = new ServiceCollection();
    services.AddApplication();
    var handlerInterfaces = GetClosedInterfaces(typeof(IRequestHandler<,>), typeof(IRequestHandler<>));

    // Act
    var registeredServiceTypes = services.Select(x => x.ServiceType).ToHashSet();

    // Assert
    Assert.NotEmpty(handlerInterfaces);

    foreach (var handlerInterface in handlerInterfaces)
    {
      Assert.Contains(handlerInterface, registeredServiceTypes);
    }
  }

  [Fact]
  public void AddApplication_AllValidatorsAreRegistered()
  {
    // Arrange
    var services = new ServiceCollection();
    services.AddApplication();
    var validatorInterfaces = GetClosedInterfaces(typeof(IValidator<>));

    // Act
    var registeredServiceTypes = services.Select(x => x.ServiceType).ToHashSet();

    // Assert
    Assert.NotEmpty(validatorInterfaces);

    foreach (var validatorInterface in validatorInterfaces)
    {
      Assert.Contains(validatorInterface, registeredServiceTypes);
    }
  }

  [Fact]
  public void AddApplication_RegistersConfiguredPipelineBehaviours()
  {
    // Arrange
    var services = new ServiceCollection();

    // Act
    services.AddApplication();

    // Assert
    Assert.Contains(services, x =>
      x.ServiceType.IsGenericType &&
      x.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) &&
      x.ImplementationType == typeof(ValidationBehaviour<,>));

    Assert.Contains(services, x =>
      x.ServiceType.IsGenericType &&
      x.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) &&
      x.ImplementationType == typeof(UnHandledExceptionBehaviour<,>));

    Assert.Contains(services, x =>
      x.ServiceType.IsGenericType &&
      x.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) &&
      x.ImplementationType == typeof(PerformanceBehaviour<,>));

    Assert.Contains(services, x =>
      x.ServiceType.IsGenericType &&
      x.ServiceType.GetGenericTypeDefinition() == typeof(IRequestPreProcessor<>) &&
      x.ImplementationType == typeof(LoggingBehaviour<>));
  }

  [Fact]
  public void AddApplication_RegistersAutoMapperAndDiscoversProfiles()
  {
    // Arrange
    var services = new ServiceCollection();
    var applicationAssembly = typeof(DependencyInjection).Assembly;
    var profileTypes = applicationAssembly
      .GetTypes()
      .Where(x => typeof(Profile).IsAssignableFrom(x) && x is { IsAbstract: false, IsClass: true })
      .ToList();

    // Act
    services.AddApplication();
    using var loggerFactory = LoggerFactory.Create(x => { });
    var config = new MapperConfiguration(x => x.AddMaps(applicationAssembly), loggerFactory);
    var mapper = config.CreateMapper();

    // Assert
    Assert.NotEmpty(profileTypes);
    Assert.Contains(services, x => x.ServiceType == typeof(IMapper));
    Assert.NotNull(mapper);
  }

  private static IReadOnlyCollection<Type> GetClosedInterfaces(params Type[] openGenericTypes)
  {
    var applicationAssembly = typeof(DependencyInjection).Assembly;

    return applicationAssembly
      .GetTypes()
      .Where(x => x is { IsClass: true, IsAbstract: false })
      .SelectMany(x => x.GetInterfaces())
      .Where(x => x.IsGenericType && openGenericTypes.Contains(x.GetGenericTypeDefinition()))
      .Distinct()
      .ToList();
  }
}