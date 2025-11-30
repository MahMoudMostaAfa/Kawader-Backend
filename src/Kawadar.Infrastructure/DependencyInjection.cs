
namespace Kawadar.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Infrastructure.Identity;

public static class DependencyInjection
{

  public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
  {
    var 

    service.AddSingleton(TimeProvider.System);


    service.AddTransient<IIdentityService, IdentityService>();
    return service;
  }

}