using Kawadar.Api;
using Kawadar.Application;
using Kawadar.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.
AddPresentation(builder.Configuration)
.AddInfrastructure(builder.Configuration)
.AddApplication();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
