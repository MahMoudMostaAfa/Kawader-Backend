using Kawadar.Api;
using Kawadar.Api.Infrastructure;
using Kawadar.Application;
using Kawadar.Infrastructure;
using Kawadar.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.
AddPresentation(builder.Configuration)
.AddInfrastructure(builder.Configuration)
.AddApplication();




var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
  // expose OpenAPI in Development environment
  app.MapOpenApi();

  // enable swagger ui in Development environment
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


//}
//else
//{
  //app.UseHsts();
//}



app.UseCoreMiddleware(builder.Configuration);


app.MapControllers();

app.Run();
