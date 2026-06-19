using Hangfire;
using Kawadar.Api;
using Kawadar.Api.Infrastructure;
using Kawadar.Application;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Infrastructure;
using Kawadar.Infrastructure.Data;
using Kawadar.Infrastructure.Hubs;
using Prometheus;
using Scalar.AspNetCore;
using Serilog;

// Configure Serilog from appsettings
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateBootstrapLogger();

try
{
  Log.Information("Starting Kawadar API...");

  var builder = WebApplication.CreateBuilder(args);

  // Use Serilog as the logging provider
  builder.Host.UseSerilog((context, services, configuration) => configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .Enrich.WithEnvironmentName()
      .Enrich.WithMachineName()
      .Enrich.WithThreadId()
      .Enrich.WithProcessId());

  builder.Services
      .AddPresentation(builder.Configuration)
      .AddInfrastructure(builder.Configuration)
      .AddApplication();




  var app = builder.Build();

  // Serilog request logging (replaces default request logging)
  app.UseSerilogRequestLogging(options =>
  {
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
      {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
      };
  });

  // Prometheus metrics
  app.UseHttpMetrics();


  // if (app.Environment.IsDevelopment())
  // {
  app.UseDeveloperExceptionPage();
  // expose OpenAPI
  app.MapOpenApi();

  // enable swagger ui
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "Kawadar Api v1");
    options.EnableDeepLinking();
    options.EnablePersistAuthorization();
    options.DisplayRequestDuration();
    options.EnableFilter();
  });

  // enable scalar 
  app.MapScalarApiReference();


  // seed database with test data
  await app.InitialiseDatabaseAsync();

  // Hangfire dashboard (development only for security)
  app.UseHangfireDashboard("/hangfire", new DashboardOptions
  {
    Authorization = [new HangfireAuthorizationFilter()]
  });

  // }

  // global middleware
  app.UseCoreMiddleware(builder.Configuration);


  // register recurring jobs
  var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
#pragma warning disable CS0618 // Type or member is obsolete
  recurringJobManager.AddOrUpdate<IPolicyViolationService>(
    "ProcessPolicyViolations",
    service => service.ProcessPolicyViolationAsync(),
    Cron.MinuteInterval(3));
#pragma warning restore CS0618 // Type or member is obsolete

  app.MapControllers().RequireRateLimiting("SlidingWindow");

  // Prometheus metrics endpoint
  app.MapMetrics();

  // signalR hubs
  app.MapHub<PersistanceHub>("/hubs/persistance");
  app.MapHub<NotificationHub>("/hubs/notifications");
  app.MapHub<ConversationHub>("/hubs/conversations");

  app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
