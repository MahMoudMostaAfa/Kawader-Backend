
namespace Kawadar.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Infrastructure.Identity;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Unicode;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Kawadar.Infrastructure.Data.Interceptors;

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

    });

    service.AddIdentityCore<AppUser>(options =>
    {
      options.Password.RequireDigit = true;
      options.Password.RequireLowercase = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequiredLength = 6;
      options.SignIn.RequireConfirmedEmail = true;
    }).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();





    service.AddAuthorizationBuilder();




    service.AddTransient<IIdentityService, IdentityService>();
    return service;
  }

}