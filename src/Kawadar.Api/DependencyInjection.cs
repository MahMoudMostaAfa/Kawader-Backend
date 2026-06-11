using System.Text.Json.Serialization;
using Asp.Versioning;
using Kawadar.Api.Infrastructure;
using Kawadar.Api.OpenApi.Transformer;
using Kawadar.Api.Services;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Caching;
using Kawadar.Infrastructure.Hubs;
using Kawadar.Infrastructure.Settings;
using Microsoft.AspNetCore.RateLimiting;

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
    .AddIdentityServices()
    .AddAppRateLimiting()
    .AddOutputCaching();

    return services;
  }


  public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddSlidingWindowLimiter("SlidingWindow",
          limiterOptions =>
          {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.SegmentsPerWindow = 6;
            limiterOptions.QueueLimit = 10;
            limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            limiterOptions.AutoReplenishment = true;
          });
      options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    });

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

  public static IServiceCollection AddOutputCaching(this IServiceCollection services)
  {
    services.AddSingleton<SharedOutputCachePolicy>();
    services.AddScoped<ICacheInvalidator, OutputCacheInvalidator>();

    services.AddOutputCache(options =>
    {
      options.AddPolicy("JobsCachePolicy", policy =>
      {
        // Override the default policy that blocks caching for authenticated requests
        policy.AddPolicy<SharedOutputCachePolicy>();

        policy.Expire(TimeSpan.FromMinutes(10));

        // All authenticated users share the same cache entries — do NOT vary by Authorization header
        policy.SetVaryByQuery(
                    "search",
                    "specilizationId",
                    "jobType",
                    "experienceLevel",
                    "budgetRange",
                    "hourlyRateRange",
                    "skillIds",
                    "page",
                    "pageSize",
                    "sortBy"
                );

        policy.Tag(CacheTags.JobsAll);
      });
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

    // 6 - Rate Limiting
    app.UseRateLimiter();

    // 7. Authentication (must come before authorization)
    app.UseAuthentication();

    // 8. Authorization (must come after authentication)
    app.UseAuthorization();

    // 9. Output Cache must come AFTER auth so the identity is established
    //    before the cache middleware decides whether to serve/store a response
    app.UseOutputCache();


    return app;
  }


}