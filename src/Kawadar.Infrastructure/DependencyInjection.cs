
namespace Kawadar.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public static class DependencyInjection
{

  public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
  {

    return service;
  }

}