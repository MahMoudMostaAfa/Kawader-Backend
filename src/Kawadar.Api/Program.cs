using Hangfire;
using Kawadar.Api;
using Kawadar.Api.Infrastructure;
using Kawadar.Application;
using Kawadar.Infrastructure;
using Kawadar.Infrastructure.Data;
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

  // expose OpenAPI
  app.MapOpenApi();

  // enable swagger ui
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "Kawadar Api v1");
    options.EnableDeepLinking();
    options.DisplayRequestDuration();
    options.EnableFilter();
  });

  // enable scalar 
  app.MapScalarApiReference();

  await app.InitialiseDatabaseAsync();

  // Hangfire dashboard (development only for security)
  app.UseHangfireDashboard("/hangfire");

  app.UseCoreMiddleware(builder.Configuration);

  app.MapControllers();

  // Prometheus metrics endpoint
  app.MapMetrics();

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
