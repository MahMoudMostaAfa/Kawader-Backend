using System.Text.Json.Serialization;
using Asp.Versioning;
using Kawadar.Api.Infrastructure;
using Kawadar.Api.OpenApi.Transformer;
using Kawadar.Api.Services;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Infrastructure.Settings;

namespace Kawadar.Api;

public static class DependencyInjection
{

  public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddCustomProblemDetails()
    .AddControllersWithJsonOptions()
    .AddGlobalExceptionHandler()
    .AddApiDocumentation()
    .AddApiVersioning()
    .AddConfiguredCors(configuration)
    .AddIdentityServices();
    return services;
  }

  public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
  {
    services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
    {
      context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
      context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
    });

    return services;
  }

  public static IServiceCollection AddControllersWithJsonOptions(this IServiceCollection services)
  {
    services.AddControllers().AddJsonOptions(
      options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      }
    );



    return services;
  }

  public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
  {
    services.AddExceptionHandler<GlobalExceptionHandler>();
    return services;
  }

  public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
  {
    string[] versions = ["v1"];

    foreach (var version in versions)
    {
      services.AddOpenApi(version, options =>
      {
        // Versioning Config 

        options.AddDocumentTransformer<VersionInfoTransformer>();


        // security scheme config
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        options.AddOperationTransformer<BearerSecuritySchemeTransformer>();


      });

    }

    return services;

  }

  public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
  {
    var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>()!;


    services.AddCors(options =>
    {
      options.AddPolicy(appSettings.CorsPolicyName, policy => policy
                .WithOrigins(appSettings.AllowedOrigins!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    return services;
  }

  public static IServiceCollection AddIdentityServices(this IServiceCollection services)
  {
    services.AddScoped<IUser, CurrentUser>();
    services.AddHttpContextAccessor();

    return services;
  }
  public static IServiceCollection AddApiVersioning(this IServiceCollection services)
  {

    services.AddApiVersioning(options =>
          {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
          })
    .AddMvc()
    .AddApiExplorer(options =>
    {
      options.GroupNameFormat = "'v'VVV";
      options.SubstituteApiVersionInUrl = true;
    });

    return services;
  }

  public static IApplicationBuilder UseCoreMiddleware(this IApplicationBuilder app, IConfiguration configuration)
  {
    // 1. Exception handling should be FIRST to catch all errors
    app.UseExceptionHandler();

    // 2. Status code pages for handling HTTP status codes
    app.UseStatusCodePages();

    // 3. HTTPS redirection (before any other middleware that might generate URLs)
    app.UseHttpsRedirection();


    // 5. CORS (before authentication/authorization)
    app.UseCors(configuration["AppSettings:CorsPolicyName"]!);



    // 7. Authentication (must come before authorization)
    app.UseAuthentication();

    // 8. Authorization (must come after authentication)
    app.UseAuthorization();



    return app;
  }

}