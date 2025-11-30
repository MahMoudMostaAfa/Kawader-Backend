
namespace Kawadar.Application;

using System.Reflection;
using FluentValidation;
using Kawadar.Application.Common.Behaviours;
using Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{

  public static IServiceCollection AddApplication(this IServiceCollection services)
  {

    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());


    services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
      cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
      cfg.AddOpenBehavior(typeof(UnHandledExceptionBehaviour<,>));
      cfg.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
      cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));

    });
    return services;
  }
}
