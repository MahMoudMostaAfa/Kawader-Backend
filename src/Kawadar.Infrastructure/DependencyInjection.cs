namespace Kawadar.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Kawadar.Infrastructure.Identity;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Kawadar.Infrastructure.Data.Interceptors;
using Kawadar.Domain.Common.Constants;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.BackgroundJobs;
using Kawadar.Infrastructure.Services;
using Kawadar.Infrastructure.Services.BackgroundJobs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Infrastructure.Services.Repositories;
using Azure.Storage.Blobs;
using Azure.Identity;
using Kawadar.Infrastructure.Services.CloudServices;
using Kawadar.Infrastructure.Services.AIServices;
using Kawadar.Application.Common.Messaging;
using Kawadar.Infrastructure.Messaging;
using MassTransit;
using Kawadar.Infrastructure.Messaging.Consumers;
using Hangfire;
using Hangfire.SqlServer;
using Kawadar.Application.Common.Hubs;
using Kawadar.Infrastructure.Services.HubServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public static class DependencyInjection
{

  public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
  {
    service.AddSingleton(TimeProvider.System);

    var connectionString = configuration.GetConnectionString("MonsterAsp");

    ArgumentException.ThrowIfNullOrEmpty(connectionString, "Connection string 'MonsterAsp' not found.");


    service.AddScoped<AuditInterceptor>();

    service.AddDbContext<AppDbContext>((sp, options) =>
    {
      var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
      options.UseSqlServer(connectionString).AddInterceptors(auditInterceptor);

    });


    service.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
      var jwtSettings = configuration.GetSection("JwtSettings");

      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
      };

      // ✅ Critical: SignalR sends the token in the query string
      options.Events = new JwtBearerEvents
      {
        OnMessageReceived = context =>
        {
          var accessToken = context.Request.Query["access_token"];
          var path = context.HttpContext.Request.Path;

          if (!string.IsNullOrEmpty(accessToken) &&
                  (path.StartsWithSegments("/hubs/messaging") ||
                   path.StartsWithSegments("/hubs/notifications") ||
                   path.StartsWithSegments("/hubs/persistance")))
          {
            context.Token = accessToken;
          }

          return Task.CompletedTask;
        }
      };

    });

    service.AddIdentityCore<AppUser>(options =>
    {
      options.Password.RequireDigit = true;
      options.Password.RequireLowercase = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequiredLength = 6;
      options.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


    var authBuilder = service.AddAuthorizationBuilder();

    // add MassTransit and rabbitMq services 
    service.AddMassTransitCfg(configuration);

    // add Hangfire services
    service.AddHangfireCfg(configuration);

    // add signalR services
    service.AddSignalRConfig();

    //Adding Azure Blob Storage
    service.AddSingleton(provider =>
    {
      var azureSection = configuration.GetSection("Azure");
      var storageConnectionString = azureSection["ConnectionString"];

      if (!string.IsNullOrWhiteSpace(storageConnectionString))
      {
        return new BlobServiceClient(storageConnectionString);
      }

      var accountStorageName = azureSection["StorageAccountName"];
      ArgumentException.ThrowIfNullOrEmpty(accountStorageName, "Account Storage Name 'Azure' not found.");
      var blobUri = new Uri($"https://{accountStorageName}.blob.core.windows.net");
      return new BlobServiceClient(blobUri, new DefaultAzureCredential());
    });


    // Automatically add policies for all permissions
    foreach (var permission in Permissions.GetAllPermissions())
    {
      authBuilder.AddPolicy(permission, policy =>
        policy.RequireClaim("Permission", permission));
    }


    service.AddScoped<ApplicationDbContextInitialiser>();
    service.AddScoped<ITokenProvider, TokenProvider>();

    // Email services
    service.AddScoped<IEmailService, EmailService>();
    service.AddSingleton<IEmailTemplateService, EmailTemplateService>();
    // AI services
    service.AddScoped<IAIService, GeminiApiService>();

    // repositories and unit of work
    service.AddScoped<IUsersRepository, UsersRepository>();
    service.AddScoped<IStorageClient, AzureStorageClient>();
    service.AddScoped<IPortfolioProjectRepository, PortfolioProjectRepository>();
    service.AddScoped<IBadgeRepository, BadgeRepository>();
    service.AddScoped<ISpecilizationRepository, SpecilizationRepository>();
    service.AddScoped<IProjectViewRepository, ProjectViewRepository>();
    service.AddScoped<ISkillRepository, SkillRepository>();
    service.AddScoped<IJobsRepository, JobsRepository>();
    service.AddScoped<IReviewRepository, ReviewRepository>();

    service.AddScoped<IJobViewRepository, JobViewRepository>();
    service.AddScoped<IProposalsRepository, ProposalsRepository>();
    service.AddScoped<IUnitOfWork, UnitOfWork>();
    service.AddTransient<IIdentityService, IdentityService>();


    // Account deletion scheduler
    service.AddScoped<IAccountDeletionScheduler, HangfireAccountDeletionScheduler>();

    return service;
  }

  public static IServiceCollection AddMassTransitCfg(this IServiceCollection services, IConfiguration configuration)
  {

    // Register EventBus abstraction
    services.AddScoped<IEventBus, EventBus>();

    services.AddMassTransit(x =>
    {
      // register all consumers
      x.AddConsumer<SendWelcomeEmailConsumer>();
      x.AddConsumer<UpdateProfileImageConsumer>();
      x.AddConsumer<UploadIdentityConsumer>();
      x.AddConsumer<ProcessingIdentityDataConsumer>();


      // rabbitMQ cfg
      x.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
        {
          h.Username(configuration["RabbitMQ:Username"]!);
          h.Password(configuration["RabbitMQ:Password"]!);
        });


        // Email queue configuration
        cfg.ReceiveEndpoint("email-welcome-queue", e =>
        {
          e.PrefetchCount = 10;

          // Retry policy: 3 times with exponential backoff
          e.UseMessageRetry(r => r.Exponential(
                      retryLimit: 3,
                      minInterval: TimeSpan.FromSeconds(5),
                      maxInterval: TimeSpan.FromMinutes(2),
                      intervalDelta: TimeSpan.FromSeconds(10)));

          // Dead letter queue after all retries fail
          e.SetQueueArgument("x-dead-letter-exchange", "email-welcome-dlx");
          e.SetQueueArgument("x-dead-letter-routing-key", "email-welcome-dlq");


          e.ConfigureConsumer<SendWelcomeEmailConsumer>(context);
        });

        // upload files queue configuration
        cfg.ReceiveEndpoint("upload-files-queue", e =>
        {
          e.PrefetchCount = 5;

          // Retry policy: 3 times with exponential backoff
          e.UseMessageRetry(r => r.Exponential(
                      retryLimit: 3,
                      minInterval: TimeSpan.FromSeconds(5),
                      maxInterval: TimeSpan.FromMinutes(2),
                      intervalDelta: TimeSpan.FromSeconds(10)));

          // Dead letter queue after all retries fail
          e.SetQueueArgument("x-dead-letter-exchange", "upload-files-dlx");
          e.SetQueueArgument("x-dead-letter-routing-key", "upload-files-dlq");


          e.ConfigureConsumer<UpdateProfileImageConsumer>(context);
          e.ConfigureConsumer<UploadIdentityConsumer>(context);
        });

        // llm processing queue configuration
        cfg.ReceiveEndpoint("llm-processing-queue", e =>
        {
          e.PrefetchCount = 5;

          // Retry policy: 3 times with exponential backoff
          e.UseMessageRetry(r => r.Exponential(
                      retryLimit: 3,
                      minInterval: TimeSpan.FromSeconds(5),
                      maxInterval: TimeSpan.FromMinutes(2),
                      intervalDelta: TimeSpan.FromSeconds(10)));

          // Dead letter queue after all retries fail
          e.SetQueueArgument("x-dead-letter-exchange", "llm-processing-dlx");
          e.SetQueueArgument("x-dead-letter-routing-key", "llm-processing-dlq");
          e.ConfigureConsumer<ProcessingIdentityDataConsumer>(context);
        });

        // Declare DLX and bind DLQ for llm processing queue
        cfg.ReceiveEndpoint("llm-processing-dlq", dlq =>
        {
          dlq.Bind("llm-processing-dlx", s =>
          {
            s.RoutingKey = "llm-processing-dlq";
            s.ExchangeType = "direct";
          });
        });

        // Declare DLX and bind DLQ
        cfg.ReceiveEndpoint("email-welcome-dlq", dlq =>
        {
          dlq.Bind("email-welcome-dlx", s =>
          {
            s.RoutingKey = "email-welcome-dlq";
            s.ExchangeType = "direct";
          });
        });

        // Declare DLX and bind DLQ for upload files queue  
        cfg.ReceiveEndpoint("upload-files-dlq", dlq =>
        {
          dlq.Bind("upload-files-dlx", s =>
          {
            s.RoutingKey = "upload-files-dlq";
            s.ExchangeType = "direct";
          });
        });




      });


    });
    return services;
  }



  public static IServiceCollection AddHangfireCfg(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("MonsterAsp");
    ArgumentException.ThrowIfNullOrEmpty(connectionString, "Connection string 'MonsterAsp' not found.");

    // Hangfire configuration
    services.AddHangfire(config => config
      .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
      .UseSimpleAssemblyNameTypeSerializer()
      .UseRecommendedSerializerSettings()
      .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
      {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
      }));
    services.AddHangfireServer();


    return services;
  }


  public static IServiceCollection AddSignalRConfig(this IServiceCollection services)
  {

    services.AddSignalR(
       opt =>
       {
         opt.EnableDetailedErrors = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>().IsDevelopment();
         opt.KeepAliveInterval = TimeSpan.FromSeconds(15);
         opt.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
       }
   );


    services.AddSingleton<IPersistanceService, PersistanceService>();

    return services;


  }
}